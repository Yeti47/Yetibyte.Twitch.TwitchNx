using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public interface ICommandSourceFactory
    {
        string Id { get; }
        string SourceDisplayName { get; }

        ICommandSourceSettings ApplySettings(ICommandSourceSettingsViewModel settingsViewModel);

        ICommandSource CreateCommandSource(IProject project);

        ICommandSourceSettingsViewModel CreateSettingsViewModel(IDirtiable parentViewModel, IProject? project);



    }
}
