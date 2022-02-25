namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public interface ICommandQueue
    {
        bool IsQueueFull { get; }

        bool Enqueue(Command command);
    }
}
