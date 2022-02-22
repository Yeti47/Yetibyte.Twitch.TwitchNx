namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{
    public class IrcMessageEventArgs : EventArgs
    {
        public IrcMessage Message { get; }

        public IrcMessageEventArgs(IrcMessage message)
        {
            Message = message;
        }

    }
}
