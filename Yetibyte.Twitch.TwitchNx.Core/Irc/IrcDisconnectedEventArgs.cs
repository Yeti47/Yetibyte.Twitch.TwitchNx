namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{
    public class IrcDisconnectedEventArgs : EventArgs
    {
        public string Message { get; init; } = string.Empty;

    }
}
