using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    public interface IMacroInstructionTemplateFactoryFacade
    {
        MacroInstructionTemplateViewModel CreateFor(IMacroInstruction macroInstruction);
    }
}
