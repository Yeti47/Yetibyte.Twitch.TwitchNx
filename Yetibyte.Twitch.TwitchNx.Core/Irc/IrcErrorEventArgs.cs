namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{
    public class IrcErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; }

        public IrcErrorEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

    }
}
