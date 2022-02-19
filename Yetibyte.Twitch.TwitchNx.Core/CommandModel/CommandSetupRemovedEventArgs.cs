namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class CommandSetupRemovedEventArgs : EventArgs
    {
        public CommandSetup CommandSetup { get; }

        public CommandSetupRemovedEventArgs(CommandSetup commandSetup)
        {
            CommandSetup = commandSetup;
        }
    }
}
