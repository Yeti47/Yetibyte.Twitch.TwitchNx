using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.Irc;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class TwitchIrcCommandSourceFactory : ICommandSourceFactory
    {
        public string SourceDisplayName => "Twitch IRC";

        public string Id => "TWITCH_IRC";

        public ICommandSourceSettings ApplySettings(ICommandSourceSettingsViewModel settingsViewModel)
        {
            IrcCommandSourceSettingsViewModel? settings = settingsViewModel as IrcCommandSourceSettingsViewModel;

            if (settings is null)
                throw new InvalidOperationException("No valid command source settings have been provided.");

            return new TwitchIrcCommandSourceSettings
            {
                AuthToken = settings.AuthToken,
                ChannelName = settings.ChannelName,
                UserName = settings.UserName
            };
        }

        public ICommandSource CreateCommandSource(IProject project)
        {
            TwitchIrcCommandSourceSettings? settings = project.ReadCommmandSourceSettings(typeof(TwitchIrcCommandSourceSettings), TwitchIrcCommandSourceSettings.COMMAND_SOURCE_ID) as TwitchIrcCommandSourceSettings;

            if (settings is null)
                throw new ArgumentException("No valid settings have been supplied for the command source.");

            TwitchIrcClient ircClient = new TwitchIrcClient();

            ircClient.Initialize(settings.UserName,settings.ChannelName,settings.AuthToken);

            var logger = log4net.LogManager.GetLogger(ApplicationConstants.ROOT_LOGGER_NAME);

            return new IrcCommandSource(ircClient, project.CommandSettings, logger);
        }

        public ICommandSourceSettingsViewModel CreateSettingsViewModel(IDirtiable parentViewModel, IProject? project)
        {
            TwitchIrcCommandSourceSettings? settings = project?.ReadCommmandSourceSettings(typeof(IrcCommandSource), TwitchIrcCommandSourceSettings.COMMAND_SOURCE_ID) as TwitchIrcCommandSourceSettings;

            return new IrcCommandSourceSettingsViewModel(parentViewModel)
            {
                AuthToken = settings?.AuthToken ?? string.Empty,
                ChannelName = settings?.ChannelName ?? string.Empty,
                UserName = settings?.UserName ?? string.Empty
            };
        }
    }

}
