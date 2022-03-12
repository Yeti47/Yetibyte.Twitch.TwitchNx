using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Styling;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    [DefaultPane("MainRight")]
    public class MacroToolBoxViewModel : Layout.ToolViewModel
    {
        private readonly ObservableCollection<MacroToolBoxItemViewModel> _simpleToolBoxItems = new ObservableCollection<MacroToolBoxItemViewModel>();
        private readonly ObservableCollection<MacroToolBoxItemViewModel> _animatedToolBoxItems = new ObservableCollection<MacroToolBoxItemViewModel>();

        public IEnumerable<MacroToolBoxItemViewModel> SimpleToolBoxItems => _simpleToolBoxItems;
        public IEnumerable<MacroToolBoxItemViewModel> AnimatedToolBoxItems => _animatedToolBoxItems;

        public MacroToolBoxViewModel(IEnumerable<MacroInstructionTemplateViewModel> macroInstructionTemplates) : base("Macro Toolbox")
        {
            foreach(var macroInstructionTemplate in macroInstructionTemplates)
            {
                MacroToolBoxItemViewModel vm = new MacroToolBoxItemViewModel(macroInstructionTemplate);

                if (macroInstructionTemplate.MacroInstructionType == Core.CommandModel.Macros.MacroInstructionType.Simple)
                {
                    _simpleToolBoxItems.Add(vm);
                }
                else
                {
                    _animatedToolBoxItems.Add(vm);
                }
            }
        }
    }
}
