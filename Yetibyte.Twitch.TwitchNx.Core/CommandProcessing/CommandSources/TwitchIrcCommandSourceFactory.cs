using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Irc;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class TwitchIrcCommandSourceFactory : ICommandSourceFactory
    {
        private ICommandSourceSettingsViewModel? _settings;

        public string SourceDisplayName => "Twitch IRC";

        public bool IsReady => _settings is not null;

        public void ApplySettings(ICommandSourceSettingsViewModel settingsViewModel)
        {
            _settings = settingsViewModel;
        }

        public ICommandSource CreateCommandSource(CommandSettings commandSettings)
        {
            IrcCommandSourceSettingsViewModel? settings = _settings as IrcCommandSourceSettingsViewModel;

            if (settings is null)
                throw new InvalidOperationException("No valid settings have been applied to the command source.");

            TwitchIrcClient ircClient = new TwitchIrcClient();

            ircClient.Initialize(settings.UserName, settings.ChannelName, settings.AuthToken);

            return new IrcCommandSource(ircClient, commandSettings);
        }

        public ICommandSourceSettingsViewModel CreateSettingsViewModel()
        {
            return new IrcCommandSourceSettingsViewModel();
        }
    }

}
