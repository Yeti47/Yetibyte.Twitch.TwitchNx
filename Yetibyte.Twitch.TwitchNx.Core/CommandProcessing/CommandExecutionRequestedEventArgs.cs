namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public class CommandExecutionRequestedEventArgs : EventArgs
    {
        public CommandReceiver.IQueueItem QueueItem { get; }

        public CommandExecutionRequestedEventArgs(CommandReceiver.IQueueItem queueItem)
        {
            QueueItem = queueItem;
        }
    }
}
