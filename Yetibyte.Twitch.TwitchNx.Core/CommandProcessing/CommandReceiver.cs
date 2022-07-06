using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using log4net;
using Yetibyte.Twitch.TwitchNx.Core.Utilities;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public class CommandReceiver : ICommandQueue, IRunnable
    {
        public enum QueueItemState
        {
            New,
            MacroStartRequested,
            MacroStarted,
            MacroComplete,
            Error,
        }

        public interface IQueueItem
        {
            string MacroMessageId { get; }
            Command Command { get; }
            QueueItemState State { get; }

            event EventHandler? Updated;
        }

        public class QueueItem : IQueueItem
        {
            private readonly CommandReceiver _commandReceiver;

            private string _macroMessageId = string.Empty;
            private QueueItemState _state = QueueItemState.New;

            public string MacroMessageId
            {
                get => _macroMessageId;
                set
                {
                    _macroMessageId = value;
                    OnUpdated();
                }
            }

            public Command Command { get; }

            public QueueItemState State
            {
                get => _state;
                set
                {
                    _state = value;
                    OnUpdated();
                }
            }

            public event EventHandler? Updated;

            public QueueItem(Command command, CommandReceiver commandReceiver)
            {
                Command = command;
                _commandReceiver = commandReceiver;
            }

            protected virtual void OnUpdated()
            {
                var handler = Updated;
                handler?.Invoke(this, EventArgs.Empty);

                _commandReceiver.RaiseQueueItemUpdatedEvent(this);
            }
        }

        public class QueueItemEventArgs : EventArgs
        {
            public CommandReceiver.IQueueItem QueueItem { get; }

            public QueueItemEventArgs(CommandReceiver.IQueueItem queueItem)
            {
                QueueItem = queueItem;
            }
        }

        private const int MACRO_START_TIMEOUT_MILLISECS = 5000;
        private const int MACRO_COMPLETE_TIMEOUT_TOLERANCE_MILLISECS = 5000;

        private readonly List<QueueItem> _commandQueue = new List<QueueItem>();
        private readonly List<QueueItem> _commandHistory = new List<QueueItem>();

        private readonly SwitchConnector _switchConnector;

        private ICommandSource? _commandSource;
        private SwitchBridgeClientConnectionSettings? _connectionSettings;

        private readonly List<CommandProcessor> _commandProcessors = new List<CommandProcessor>();
        private readonly ILog? _logger;

        public event EventHandler? Started;
        public event EventHandler? Stopped;

        public event EventHandler<QueueItemEventArgs>? QueueItemAdded;
        public event EventHandler<QueueItemEventArgs>? QueueItemRemoved;
        public event EventHandler<QueueItemEventArgs>? QueueItemUpdated;

        public event EventHandler<QueueItemEventArgs?> QueueItemAddedToHistory;
        public event EventHandler<QueueItemEventArgs?> QueueItemRemovedFromHistory;

        public event EventHandler<CommandExecutionRequestedEventArgs>? CommandExecutionRequested;

        public ICommandSource? CommandSource => _commandSource;

        public bool IsRunning => _commandSource?.IsRunning ?? false;

        public bool IsQueueFull => _commandQueue.Count >= CommandSettings.MaxQueueCapacity;

        public CommandSettings CommandSettings { get; private set; }

        public IQueueItem? CurrentCommandQueueItem => GetCurrentQueueItem();

        public bool IsInitialized { get; private set; } = false;

        public CommandReceiver(SwitchConnector switchConnector, ILog? logger = null)
        {
            _switchConnector = switchConnector;
            _logger = logger;

            CommandSettings = new CommandSettings();

        }

        public void Initialize(ICommandSource commandSource, SwitchBridgeClientConnectionSettings connectionSettings, CommandSettings commandSettings)
        {
            _commandSource = commandSource;

            CommandSettings = commandSettings;

            _connectionSettings = connectionSettings;

            _commandSource.Started += commandSource_Started;
            _commandSource.Stopped += commandSource_Stopped;
            _commandSource.CommandReceived += commandSource_CommandReceived;

            _switchConnector.MacroExecuted += switchConnector_MacroExecuted;
            _switchConnector.MacroComplete += switchConnector_MacroComplete;

            InitializeCommandProcessors();

            IsInitialized = true;   
        }

        public void Destroy()
        {
            if (_commandSource is not null)
            {
                _commandSource.Started -= commandSource_Started;
                _commandSource.Stopped -= commandSource_Stopped;
                _commandSource.CommandReceived -= commandSource_CommandReceived;
            }

            _switchConnector.MacroExecuted -= switchConnector_MacroExecuted;
            _switchConnector.MacroComplete -= switchConnector_MacroComplete;

            IsInitialized = false;
        }

        private void switchConnector_MacroComplete(object? sender, SwitchConnectorMacroCompleteEventArgs e)
        {
            QueueItem? queueItem = _commandQueue.FirstOrDefault(qi => qi.MacroMessageId == e.OriginalMacroMessageId);

            if (queueItem is null)
            {
                return;
            }

            queueItem.State = QueueItemState.MacroComplete;

            _commandQueue.Remove(queueItem);
            OnQueueItemRemoved(queueItem);  

            if (!_commandHistory.Contains(queueItem))
            {
                _commandHistory.Add(queueItem);
                OnQueueItemAddedToHistory(queueItem);
            }

            ProcessCurrentCommand();
        }

        private void switchConnector_MacroExecuted(object? sender, SwitchConnectorMacroEventargs e)
        {
            QueueItem? queueItem = _commandQueue.FirstOrDefault(qi => qi.MacroMessageId == e.MacroMessageId);

            if (queueItem is null)
            {
                return;
            }

            queueItem.State = QueueItemState.MacroStarted;

            int macroDurationMillisecs = (int)Math.Ceiling(queueItem.Command.CommandSetup.Macro.TotalSeconds * 1000);

            // If after the expected macro duration plus x seconds the item is still not in state MacroComplete,
            // something must have gone wrong.
            Task.Delay(macroDurationMillisecs + MACRO_COMPLETE_TIMEOUT_TOLERANCE_MILLISECS).ContinueWith(t =>
            {
                if (queueItem.State != QueueItemState.MacroComplete)
                {
                    queueItem.State = QueueItemState.Error;

                    _logger?.Error($"Macro completion for command '{queueItem.Command.Name}' timed out (ID: {queueItem.Command.Id}).");

                    _commandQueue.Remove(queueItem);
                    OnQueueItemRemoved(queueItem);

                    if (!_commandHistory.Contains(queueItem))
                    {
                        _commandHistory.Add(queueItem);
                        OnQueueItemAddedToHistory(queueItem);
                    }
                }
            });

        }

        public void ClearQueue()
        {
            _commandQueue.MutableForeach(qi =>
            {
                _commandQueue.Remove(qi);
                OnQueueItemRemoved(qi, true);
            });
        }

        public void ClearHistory()
        {
            _commandHistory.MutableForeach(qh =>
            {
                _commandHistory.Remove(qh);
                OnQueueItemRemovedFromHistory(qh);
            });
        }

        private void InitializeCommandProcessors()
        {
            _commandProcessors.Clear();

            foreach(CommandSetup commandSetup in CommandSettings.CommandSetups)
            {
                CommandProcessor commandProcessor = new CommandProcessor(commandSetup, this);

                _commandProcessors.Add(commandProcessor);
            }
        }

        private void commandSource_CommandReceived(object? sender, CommandReceivedEventArgs e)
        {
            CommandProcessor? matchingProcessor = _commandProcessors.FirstOrDefault(cp => cp.AppliesTo(e.Command));

            if (matchingProcessor is null)
            {
                _logger?.Info($"No matching command processor found for command '{e.Command.Name}'.");
                return;
            }

            matchingProcessor.Process(e.Command);
        }

        private void commandSource_Stopped(object? sender, EventArgs e)
        {
            OnStopped();
        }

        private void commandSource_Started(object? sender, EventArgs e)
        {
            OnStarted();
        }

        protected virtual void OnStarted()
        {
            _logger?.Info("Command Receiver started.");

            var handler = Started;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStopped()
        {
            _logger?.Info("Command Receiver stopped.");

            var handler = Stopped;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCommandExecutionRequested(QueueItem queueItem)
        {
            var handler = CommandExecutionRequested;
            handler?.Invoke(this, new CommandExecutionRequestedEventArgs(queueItem));
        }

        public bool Start()
        {
            if (IsRunning || _connectionSettings is null)
                return false;

            if (!IsInitialized)
            {
                _logger?.Error("Cannot start command receiver since it has not been initialized.");
                return false;
            }

            bool success = _commandSource?.Start() ?? false;

            if (success && !_switchConnector.IsConnected)
            {
                success = _switchConnector.ConnectAsync(_connectionSettings);
            }

            return success;
        }

        public bool Stop()
        {
            if (!IsRunning)
                return false;

            bool success = _commandSource?.Stop() ?? false;

            return success;
        }

        private QueueItem? GetCurrentQueueItem()
        {
            return _commandQueue.MinBy(c => c.Command.Timestamp);
        }

        public bool Enqueue(Command command)
        {
            _logger?.Info($"Trying to enqueue command '{command.Name}' from user {command.User.DisplayName}'...");

            if (IsQueueFull)
            {
                _logger?.Info($"Queue already full. Cannot enqueue command '{command.Name}' by user {command.User.DisplayName}.");

                return false;
            }

            var queueItem = new QueueItem(command, this);

            _commandQueue.Add(queueItem);
            OnQueueItemAdded(queueItem);

            _logger?.Info($"Command '{command.Name}' enqueued.");

            if (_commandQueue.Count == 1)
            {
                ProcessCurrentCommand();
            }

            return true;
           
        }

        private void ProcessCurrentCommand()
        {
            var queueItem = GetCurrentQueueItem();

            if (queueItem is null || queueItem.State != QueueItemState.New)
                return;

            if (_switchConnector.Controllers.Count < queueItem.Command.CommandSetup.ControllerIndex)
            {
                _logger?.Error($"Cannot process command '{queueItem.Command.Name}'. No controller with index {queueItem.Command.CommandSetup.ControllerIndex} available.");
                return;
            }

            int controllerId = _switchConnector.Controllers[queueItem.Command.CommandSetup.ControllerIndex].Id;

            try
            {
                queueItem.MacroMessageId = _switchConnector.ExecuteMacro(queueItem.Command.CommandSetup.Macro.Build(), controllerId);
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error processing command '{queueItem.Command.Name}'. Details: {ex.Message}.");

                _commandQueue.Remove(queueItem);
                OnQueueItemRemoved(queueItem);
                return;
            }

            _logger?.Info($"Requested macro execution for command '{queueItem.Command.Name}' by user {queueItem.Command.User.DisplayName}.");

            OnCommandExecutionRequested(queueItem);

            // If after X seconds, the state of the queue item is still "MacroStartRequested", something must have gone
            // wrong, so set the state to error and remove the item from the queue.
            Task.Delay(MACRO_START_TIMEOUT_MILLISECS).ContinueWith(t =>
            {
                if (queueItem.State == QueueItemState.MacroStartRequested || queueItem.State == QueueItemState.New)
                {
                    queueItem.State = QueueItemState.Error;

                    _logger?.Error($"Execution of command '{queueItem.Command.Name}' timed out (ID: {queueItem.Command.Id}).");

                    _commandQueue.Remove(queueItem);

                    OnQueueItemRemoved(queueItem);

                    if (!_commandHistory.Contains(queueItem))
                    {
                        _commandHistory.Add(queueItem);
                        OnQueueItemAddedToHistory(queueItem);
                    }
                }
            });

            queueItem.State = QueueItemState.MacroStartRequested;
            
        }

        protected virtual void OnQueueItemAdded(QueueItem queueItem)
        {
            var handler = QueueItemAdded;
            handler?.Invoke(this, new QueueItemEventArgs(queueItem));
        }

        protected virtual void OnQueueItemAddedToHistory(QueueItem queueItem)
        {
            var handler = QueueItemAddedToHistory;
            handler?.Invoke(this, new QueueItemEventArgs(queueItem));
        }

        protected virtual void OnQueueItemRemovedFromHistory(QueueItem queueItem)
        {
            var handler = QueueItemRemovedFromHistory;
            handler?.Invoke(this, new QueueItemEventArgs(queueItem));
        }

        protected virtual void OnQueueItemRemoved(QueueItem queueItem, bool skipProcessingNextItem = false)
        {
            var handler = QueueItemRemoved;
            handler?.Invoke(this, new QueueItemEventArgs(queueItem));

            if (!skipProcessingNextItem)
                ProcessCurrentCommand();
        }

        private void RaiseQueueItemUpdatedEvent(QueueItem queueItem)
        {
            var handler = QueueItemUpdated;
            handler?.Invoke(this, new QueueItemEventArgs(queueItem));
        }

    }
}
