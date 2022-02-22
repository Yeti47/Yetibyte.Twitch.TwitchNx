namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public class CommandReceivedEventArgs : EventArgs
    {
        public Command Command { get; }

        public CommandReceivedEventArgs(Command command)
        {
            Command = command;
        }
    }
