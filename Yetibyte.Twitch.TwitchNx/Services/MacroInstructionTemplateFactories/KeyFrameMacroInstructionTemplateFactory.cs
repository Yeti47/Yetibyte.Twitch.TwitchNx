using System;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using static Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroInstructionTemplateViewModel;

namespace Yetibyte.Twitch.TwitchNx.Services.MacroInstructionTemplateFactories
{
    public class KeyFrameMacroInstructionTemplateFactory : IMacroInstructionTemplateFactory
    {
        public bool AppliesTo(IMacroInstruction macroInstruction) => macroInstruction is KeyFrameMacroInstruction;

        public MacroInstructionTemplateViewModel CreateFor(IMacroInstruction macroInstruction)
        {
            if (macroInstruction is not KeyFrameMacroInstruction keyFrameMacroInstruction)
                throw new ArgumentException($"The given macro instruction must be of type {nameof(KeyFrameMacroInstruction)}.", nameof(macroInstruction));

            return new MacroInstructionTemplateViewModel(
                Array.Empty<AnimationFrame>(),
                MacroInstructionType.Simple,
                () => new KeyFrameMacroInstruction()
            );
        }
    }
}
