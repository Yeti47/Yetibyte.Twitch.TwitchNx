using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Irc;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class IrcCommandSourceFactory : ICommandSourceFactory
    {
        private readonly IIrcClient _ircClient;

        public string SourceDisplayName { get; set; }

        public IrcCommandSourceFactory(string sourceDisplayName, IIrcClient ircClient)
        {
            SourceDisplayName = sourceDisplayName;
            _ircClient = ircClient;
        }

        public ICommandSource CreateCommandSource(CommandSettings commandSettings, object settingsViewModel)
        {
            if (settingsViewModel is not IrcCommandSourceSettings settings)
                throw new ArgumentException($"Argument must be of type {nameof(IrcCommandSourceSettings)}.");

            settings.IrcClient.Initialize(settings.UserName, settings.ChannelName, settings.AuthToken);

            return new IrcCommandSource(settings.IrcClient, commandSettings);
        }

        public object CreateSettingsViewModel()
        {
            return new IrcCommandSourceSettings(_ircClient);
        }
    }
}
