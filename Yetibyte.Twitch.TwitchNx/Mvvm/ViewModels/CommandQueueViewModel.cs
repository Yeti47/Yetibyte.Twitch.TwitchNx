using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            public void NotifyPropertyChanged() => OnPropertyChanged();

        }

        private readonly CommandReceiver _commandReceiver;

        private readonly ObservableCollection<CommandQueueItemViewModel> _queueItems = new ObservableCollection<CommandQueueItemViewModel>();

        public IEnumerable<CommandQueueItemViewModel> QueueItems => _queueItems;

        public CommandQueueViewModel(CommandReceiver commandReceiver) : base("Command Queue")
        {
            _commandReceiver = commandReceiver;

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
