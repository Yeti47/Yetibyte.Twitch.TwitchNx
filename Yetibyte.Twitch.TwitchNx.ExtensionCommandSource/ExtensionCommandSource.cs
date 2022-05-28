using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource
{
    public class ExtensionCommandSource : ICommandSource
    {
        public bool IsRunning => false;

        public event EventHandler<CommandReceivedEventArgs>? CommandReceived;
        public event EventHandler? Started;
        public event EventHandler? Stopped;

        public void NotifyCommandProcessed(CommandProcessingResult commandProcessingResult)
        {
            
        }

        public bool Start()
        {
            return false;
        }

        public bool Stop()
        {
            return false;
        }
    }
}
