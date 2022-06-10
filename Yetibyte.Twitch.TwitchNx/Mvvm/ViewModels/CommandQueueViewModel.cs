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
    [DefaultPane("MainBottom")]
    public class CommandQueueViewModel : ToolViewModel
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

        private readonly CommandReceiver _commandReceiver;

        private readonly ObservableCollection<CommandQueueItemViewModel> _queueItems = new ObservableCollection<CommandQueueItemViewModel>();

        private readonly RelayCommand _clearCommand;

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

        public CommandQueueViewModel(CommandReceiver commandReceiver) : base("Command Queue")
        {
            _commandReceiver = commandReceiver;

            _commandReceiver.QueueItemAdded += _commandReceiver_QueueItemAdded;
            _commandReceiver.QueueItemRemoved += _commandReceiver_QueueItemRemoved;
            _commandReceiver.QueueItemUpdated += _commandReceiver_QueueItemUpdated;

            _clearCommand = new RelayCommand(_commandReceiver.ClearQueue);
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
                App.Current.Dispatcher.Invoke(() => _queueItems.Remove(itemVm));
            }
        }

        private void _commandReceiver_QueueItemAdded(object? sender, CommandReceiver.QueueItemEventArgs e)
        {
            CommandQueueItemViewModel commandQueueItemViewModel = new CommandQueueItemViewModel(e.QueueItem);

            App.Current.Dispatcher.Invoke(() => _queueItems.Add(commandQueueItemViewModel));
            
        }
    }
}
