using System;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine;
using static Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroInstructionTemplateViewModel;

namespace Yetibyte.Twitch.TwitchNx.Services.MacroInstructionTemplateFactories
{
    public class FixedStickDirectionMacroTemplateViewModelFactory : IMacroInstructionTemplateFactory
    {
        public bool AppliesTo(IMacroInstruction macroInstruction) => macroInstruction is FixedStickDirectionMacroInstruction;

        public MacroInstructionTemplateViewModel CreateFor(IMacroInstruction macroInstruction)
        {
            if (macroInstruction is not FixedStickDirectionMacroInstruction stickDirectionMacroInstruction)
                throw new ArgumentException($"The given macro instruction must be of type {nameof(FixedStickDirectionMacroInstruction)}.", nameof(macroInstruction));

            string imagePath = stickDirectionMacroInstruction.Stick == Core.CommandModel.ControllerStick.Left ? "Images/Macros/stick_L/Joy_L00.png" : "Images/Macros/stick_R/Joy_R00.png";

            AnimationFrame singleAnimationFrame = new AnimationFrame(imagePath, 1f);

            return new MacroInstructionTemplateViewModel(
                new[] { singleAnimationFrame },
                MacroInstructionType.Simple, 
                opt => new FixedStickDirectionMacroInstruction(
                    new FixedStickDirectionInput { 
                        Stick = stickDirectionMacroInstruction.Stick,
                        StickDirection = (opt as FixedStickDirectionMacroOptionsViewModel)?.SelectedDirection?.ControllerStickDirection ?? Core.CommandModel.ControllerStickDirection.North,
                        Pressure = (opt as FixedStickDirectionMacroOptionsViewModel)?.NormalizedPressure ?? 1f
                    }
                ),
                imagePath, 
                mi => new FixedStickDirectionMacroOptionsViewModel(
                    (mi as FixedStickDirectionMacroInstruction)?.FixedStickDirectionInput?.StickDirection ?? Core.CommandModel.ControllerStickDirection.North,
                    (mi as FixedStickDirectionMacroInstruction)?.FixedStickDirectionInput?.Pressure ?? 1f
                )
            );
        }
    }
}
