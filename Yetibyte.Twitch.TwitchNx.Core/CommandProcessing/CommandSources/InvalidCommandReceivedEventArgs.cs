namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class InvalidCommandReceivedEventArgs : EventArgs
    {
        public string Command { get; }
        public string Message { get; }
        public ICommandUser User { get; }

        public InvalidCommandReceivedEventArgs(string command, string message, ICommandUser user)
        {
            Command = command;
            Message = message;
            User = user;
        }
    }
}
