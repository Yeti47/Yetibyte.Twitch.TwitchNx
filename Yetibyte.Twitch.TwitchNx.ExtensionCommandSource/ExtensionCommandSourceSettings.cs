using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource
{
    public class ExtensionCommandSourceSettings : ICommandSourceSettings
    {
        public Type CommandSourceType => typeof(ExtensionCommandSource);

        public string CommandSourceId => nameof(ExtensionCommandSource);
    }
}
