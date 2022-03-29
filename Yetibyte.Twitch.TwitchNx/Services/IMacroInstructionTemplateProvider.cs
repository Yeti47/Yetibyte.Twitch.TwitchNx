using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    public interface IMacroInstructionTemplateProvider
    {
        IEnumerable<MacroInstructionTemplateViewModel> GetMacroInstructionTemplates();
    }
}
