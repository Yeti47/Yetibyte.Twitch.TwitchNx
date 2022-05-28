using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource
{
    public class ExtensionCommandSourceSettingsViewModel : ICommandSourceSettingsViewModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
