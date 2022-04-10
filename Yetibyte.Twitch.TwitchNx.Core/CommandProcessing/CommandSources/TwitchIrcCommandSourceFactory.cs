using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Irc;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class TwitchIrcCommandSourceFactory : ICommandSourceFactory
    {
        public string SourceDisplayName => "Twitch IRC";

        public ICommandSource CreateCommandSource(CommandSettings commandSettings, ICommandSourceSettingsViewModel settingsViewModel)
        {
            if (settingsViewModel is not IrcCommandSourceSettingsViewModel settings)
                throw new ArgumentException($"Argument must be of type {nameof(IrcCommandSourceSettingsViewModel)}.");

            settings.IrcClient.Initialize(settings.UserName, settings.ChannelName, settings.AuthToken);

            return new IrcCommandSource(settings.IrcClient, commandSettings);
        }

        public ICommandSourceSettingsViewModel CreateSettingsViewModel()
        {
            return new IrcCommandSourceSettingsViewModel(new TwitchIrcClient());
        }
    }

}
