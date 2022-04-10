using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public interface ICommandSourceFactory
    {
        string SourceDisplayName { get; }

        ICommandSource CreateCommandSource(CommandSettings commandSettings, ICommandSourceSettingsViewModel settingsViewModel);

        ICommandSourceSettingsViewModel CreateSettingsViewModel();

    }
}
