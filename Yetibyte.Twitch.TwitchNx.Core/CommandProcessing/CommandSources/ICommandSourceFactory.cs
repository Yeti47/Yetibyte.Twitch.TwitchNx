using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public interface ICommandSourceFactory
    {
        string SourceDisplayName { get; }
        bool IsReady { get; }


        void ApplySettings(ICommandSourceSettingsViewModel settingsViewModel);

        ICommandSource CreateCommandSource(CommandSettings commandSettings);

        ICommandSourceSettingsViewModel CreateSettingsViewModel();

    }
}
