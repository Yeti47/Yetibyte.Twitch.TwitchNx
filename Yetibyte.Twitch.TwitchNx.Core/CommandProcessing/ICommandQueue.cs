namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public interface ICommandQueue
    {
        bool Enqueue(Command command);
    }
}
