using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource
{
    public class ExtensionCommandSourceFactory : ICommandSourceFactory
    {
        public string Id => nameof(ExtensionCommandSourceFactory);

        public string SourceDisplayName => "TwitchNX Extension";

        public ICommandSourceSettings ApplySettings(ICommandSourceSettingsViewModel settingsViewModel)
        {
            return new ExtensionCommandSourceSettings();
        }

        public ICommandSource CreateCommandSource(IProject project)
        {
            return new ExtensionCommandSource();
        }

        public ICommandSourceSettingsViewModel CreateSettingsViewModel(IDirtiable parentViewModel, IProject? project)
        {
            return new ExtensionCommandSourceSettingsViewModel();
        }
    }
}
