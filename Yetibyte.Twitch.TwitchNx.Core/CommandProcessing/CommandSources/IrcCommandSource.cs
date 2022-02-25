using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Irc;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class IrcCommandSource : ICommandSource
    {
        private readonly IIrcClient _ircClient;
        private readonly CommandSettings _commandSettings;
        private readonly ILogger<ICommandSource>? _logger;

        public bool IsRunning { get; private set; }

        public event EventHandler<CommandReceivedEventArgs>? CommandReceived;
        public event EventHandler<InvalidCommandReceivedEventArgs>? InvalidCommandReceived;
        public event EventHandler? Started;
        public event EventHandler? Stopped;

        public IrcCommandSource(IIrcClient ircClient, CommandSettings commandSettings, ILogger<ICommandSource>? logger = null)
        {
            _ircClient = ircClient;
            _commandSettings = commandSettings;
            _logger = logger;
            _ircClient.MessageReceived += ircClient_MessageReceived;
            _ircClient.ErrorOccurred += ircClient_ErrorOccurred;
            _ircClient.ConnectionErrorOccurred += ircClient_ConnectionErrorOccurred;
            _ircClient.Connected += ircClient_Connected;
            _ircClient.Disconnected += ircClient_Disconnected;
        }

        private void ircClient_Disconnected(object? sender, IrcDisconnectedEventArgs e)
        {
            _logger?.LogInformation("IRC client disconnected.");
        }

        private void ircClient_Connected(object? sender, IrcConnectedEventArgs e)
        {
            _logger?.LogInformation($"IRC client successfully connected to channel '{e.Channel}'.");
        }

        private void ircClient_ConnectionErrorOccurred(object? sender, IrcErrorEventArgs e)
        {
            _logger?.LogError($"IRC connection error. Details: {e.ErrorMessage}");
        }

        private void ircClient_ErrorOccurred(object? sender, IrcErrorEventArgs e)
        {
            _logger?.LogError($"The IRC client threw an error. Details: {e.ErrorMessage}");
        }

        private void ircClient_MessageReceived(object? sender, IrcMessageEventArgs e)
        {
            string message = e.Message.Content.Trim();

            string[] words = message.Split(' ');

            if (!words.Any())
            {
                _logger?.LogWarning("Unable to process empty message.");
                return;
            }

            string commandWord = words.First().Trim();

            if (commandWord.StartsWith(_commandSettings.CommandPrefix))
            {
                string commandName = commandWord.Substring(_commandSettings.CommandPrefix.Length);

                CommandSetup? commandSetup = _commandSettings.CommandSetups.FirstOrDefault(x => x.Name == commandName);

                if (commandSetup is null)
                {
                    _logger?.LogInformation($"Unrecognized command '{commandWord}' received from user '{e.Message.Author.DisplayName}'.");

                    OnInvalidCommandReceived(new InvalidCommandReceivedEventArgs(commandName, message, e.Message.Author));
                    
                    return;
                }

                string commandId = Guid.NewGuid().ToString();

                Command command = new Command(commandId, e.Message.Author, commandSetup, message, e.Message.Timestamp);

                _logger?.LogInformation($"Received command '{commandWord}' from user '{e.Message.Author.DisplayName}'. Command-ID: {commandId}");

                CommandReceivedEventArgs commandReceivedEventArgs = new CommandReceivedEventArgs(command);

                OnCommandReceived(commandReceivedEventArgs);
            }
        }

        public bool Start()
        {
            if (IsRunning)
                return false;

            IsRunning = true;

            bool success = false;

            try
            {
                success = _ircClient.Connect();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred trying to connect to the IRC.");
            }

            if (success)
            {
                _logger?.LogInformation("IRC Command Source started.");

                OnStarted();
            }
            
            return success;
        }

        public bool Stop()
        {
            if (!IsRunning)
                return false;

            IsRunning = false;

            bool success = _ircClient.Disconnect();

            if (success)
            {
                _logger?.LogInformation("IRC Command Source stopped.");

                OnStopped();
            }
            else
            {
                _logger?.LogError("Could not stop IRC client.");
            }

            return success;
        }

        protected virtual void OnStarted()
        {
            var handler = Started;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStopped()
        {
            var handler = Stopped;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCommandReceived(CommandReceivedEventArgs eventArgs)
        {
            var handler = CommandReceived;
            handler?.Invoke(this, eventArgs);
        }
        protected virtual void OnInvalidCommandReceived(InvalidCommandReceivedEventArgs eventArgs)
        {
            var handler = InvalidCommandReceived;
            handler?.Invoke(this, eventArgs);
        }

        public void NotifyCommandProcessed(CommandProcessingResult commandProcessingResult)
        {
            if (commandProcessingResult.Success)
            {
                if (!commandProcessingResult.WasEnqueued)
                {
                    _logger?.LogInformation($"Informing user '{commandProcessingResult.Command.User.DisplayName}' about queue being full.");

                    _ircClient.SendMessage(_commandSettings.GetQueueFullMessage(commandProcessingResult.Command.User.DisplayName));
                }

            }
            else if(commandProcessingResult.SharedTimeRemaining > TimeSpan.Zero)
            {
                try
                {
                    string sharedCooldownMessage = commandProcessingResult.Command.CommandSetup.CooldownGroup?
                        .GetSharedCooldownMessage(commandProcessingResult.Command.User.DisplayName, commandProcessingResult.SharedTimeRemaining.TotalSeconds) 
                        ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(sharedCooldownMessage))
                    {
                        _ircClient.SendMessage(sharedCooldownMessage);
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "An error occurred while trying to send a cooldown message.");
                }
            }
            else if(commandProcessingResult.TimeRemaining > TimeSpan.Zero)
            {
                try
                {
                    string userCooldownMessage = commandProcessingResult.Command.CommandSetup.CooldownGroup?
                        .GetUserCooldownMessage(commandProcessingResult.Command.User.DisplayName, commandProcessingResult.TimeRemaining.TotalSeconds)
                        ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(userCooldownMessage))
                    {
                        _ircClient.SendMessage(userCooldownMessage);
                    }

                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "An error occurred while trying to send a cooldown message.");
                }
            }
        }
    }
}
