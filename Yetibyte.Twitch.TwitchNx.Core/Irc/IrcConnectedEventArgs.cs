namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{
    public class IrcConnectedEventArgs : EventArgs
    {
        public string Channel { get; }

        public IrcConnectedEventArgs(string channel)
        {
            Channel = channel;
        }
    }
}
