using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Styling;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public abstract class CommandQueueViewModelBase : ToolViewModel
    {
        public class CommandQueueItemViewModel : ObservableObject
        {
            public CommandReceiver.IQueueItem QueueItem { get; }

            public CommandQueueItemViewModel(CommandReceiver.IQueueItem queueItem)
            {
                QueueItem = queueItem;
            }

            public void NotifyPropertyChanged() => OnPropertyChanged(string.Empty);

        }

        protected readonly CommandReceiver _commandReceiver;

        protected readonly ObservableCollection<CommandQueueItemViewModel> _queueItems = new ObservableCollection<CommandQueueItemViewModel>();

        protected readonly RelayCommand _clearCommand;


        private System.Drawing.Color _backgroundColor = System.Drawing.Color.LimeGreen;
        private System.Drawing.Color _foregroundColor = System.Drawing.Color.Black;

        private bool _isSettingsOpen = false;

        public override bool DisableDuringSession => false;

        public IEnumerable<CommandQueueItemViewModel> QueueItems => _queueItems;

        public ICommand ClearCommand => _clearCommand;

        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set
            {
                _isSettingsOpen = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSettingsClosed));
            }
        }

        public bool IsSettingsClosed => !IsSettingsOpen;

        public System.Drawing.Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public System.Drawing.Color ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                _foregroundColor = value;
                OnPropertyChanged();
            }
        }

        public CommandQueueViewModelBase(string toolName, CommandReceiver commandReceiver) : base(toolName)
        { 
            _commandReceiver = commandReceiver;

            _clearCommand = new RelayCommand(ExecuteClearCommand);
        }

        protected abstract void ExecuteClearCommand();
    }

    [DefaultPane("MainBottom")]
    public class CommandQueueViewModel : CommandQueueViewModelBase
    {

        public CommandQueueViewModel(CommandReceiver commandReceiver) : base("Command Queue", commandReceiver)
        {
            _commandReceiver.QueueItemAdded += _commandReceiver_QueueItemAdded;
            _commandReceiver.QueueItemRemoved += _commandReceiver_QueueItemRemoved;
            _commandReceiver.QueueItemUpdated += _commandReceiver_QueueItemUpdated;
        }

        private void _commandReceiver_QueueItemUpdated(object? sender, CommandReceiver.QueueItemEventArgs e)
        {
            if (_queueItems.FirstOrDefault(qi => qi.QueueItem == e.QueueItem) is CommandQueueItemViewModel itemVm)
            {
                itemVm.NotifyPropertyChanged();
            }
        }

        private void _commandReceiver_QueueItemRemoved(object? sender, CommandReceiver.QueueItemEventArgs e)
        {
            if (_queueItems.FirstOrDefault(qi => qi.QueueItem == e.QueueItem) is CommandQueueItemViewModel itemVm)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => _queueItems.Remove(itemVm));
            }
        }

        private void _commandReceiver_QueueItemAdded(object? sender, CommandReceiver.QueueItemEventArgs e)
        {
            CommandQueueItemViewModel commandQueueItemViewModel = new CommandQueueItemViewModel(e.QueueItem);

            System.Windows.Application.Current.Dispatcher.Invoke(() => _queueItems.Add(commandQueueItemViewModel));

        }

        protected override void ExecuteClearCommand() => _commandReceiver.ClearQueue();

    }

    [DefaultPane("MainBottom")]
    public class CommandHistoryViewModel : CommandQueueViewModelBase
    {
        public CommandHistoryViewModel(CommandReceiver commandReceiver) : base("Command History", commandReceiver)
        {
            _commandReceiver.QueueItemAddedToHistory += _commandReceiver_QueueItemAddedToHistory;
            _commandReceiver.QueueItemRemovedFromHistory += _commandReceiver_QueueItemRemovedFromHistory;
            _commandReceiver.QueueItemUpdated += _commandReceiver_QueueItemUpdated;
        }

        private void _commandReceiver_QueueItemRemovedFromHistory(object? sender, CommandReceiver.QueueItemEventArgs? e)
        {
            if (_queueItems.FirstOrDefault(qi => qi.QueueItem == e.QueueItem) is CommandQueueItemViewModel itemVm)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => _queueItems.Remove(itemVm));
            }
        }

        private void _commandReceiver_QueueItemAddedToHistory(object? sender, CommandReceiver.QueueItemEventArgs? e)
        {
            CommandQueueItemViewModel commandQueueItemViewModel = new CommandQueueItemViewModel(e.QueueItem);

            System.Windows.Application.Current.Dispatcher.Invoke(() => _queueItems.Add(commandQueueItemViewModel));
        }

        private void _commandReceiver_QueueItemUpdated(object? sender, CommandReceiver.QueueItemEventArgs e)
        {
            if (_queueItems.FirstOrDefault(qi => qi.QueueItem == e.QueueItem) is CommandQueueItemViewModel itemVm)
            {
                itemVm.NotifyPropertyChanged();
            }
        }

        protected override void ExecuteClearCommand() => _commandReceiver.ClearHistory();
    }
}
