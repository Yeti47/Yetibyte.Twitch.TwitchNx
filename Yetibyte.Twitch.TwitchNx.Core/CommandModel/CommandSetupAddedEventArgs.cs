namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class CommandSetupAddedEventArgs : EventArgs
    {
        public CommandSetup CommandSetup { get; }

        public CommandSetupAddedEventArgs(CommandSetup commandSetup)
        {
            CommandSetup = commandSetup;
        }
    }
}
