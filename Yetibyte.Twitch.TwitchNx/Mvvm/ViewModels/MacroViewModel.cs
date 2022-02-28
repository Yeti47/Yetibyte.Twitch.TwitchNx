using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MacroInstructionViewModel : ObservableObject
    {

    }

    public class ControllerInputViewModel : ObservableObject
    {

    }

    public class MacroViewModel : ObservableObject
    {
        

        private readonly Macro _macro;

        public MacroViewModel(Macro macro)
        {
            _macro = macro;
        }
    }
}
