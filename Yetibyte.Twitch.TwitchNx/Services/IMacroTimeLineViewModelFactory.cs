using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    public interface IMacroTimeLineViewModelFactory
    {
        MacroTimeLineViewModel CreateViewModel(Macro macro);
    }
}
