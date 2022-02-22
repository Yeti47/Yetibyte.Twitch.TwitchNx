namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public interface ICommandSource
    {
        event EventHandler<CommandReceivedEventArgs>? CommandReceived;
        event EventHandler? Started;
        event EventHandler? Stopped;

        bool IsRunning { get; }

        bool Run();

        bool Stop();

    }
}
