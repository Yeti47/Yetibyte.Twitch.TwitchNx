using System;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    public interface IMacroInstructionTemplateFactory
    {
        bool AppliesTo(IMacroInstruction macroInstruction);

        MacroInstructionTemplateViewModel CreateFor(IMacroInstruction macroInstruction);
    }
}
