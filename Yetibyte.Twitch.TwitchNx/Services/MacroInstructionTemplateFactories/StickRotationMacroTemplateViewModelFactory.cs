using System;
using System.Collections.Generic;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine;
using static Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroInstructionTemplateViewModel;

namespace Yetibyte.Twitch.TwitchNx.Services.MacroInstructionTemplateFactories
{
    public class StickRotationMacroTemplateViewModelFactory : IMacroInstructionTemplateFactory
    {
        public const string ANIMATION_FRAME_IMAGE_FILE_EXTENSION = "png";

        private const float STICK_ROTATION_FRAME_DURATION_SHORT = 0.04f;
        private const float STICK_ROTATION_FRAME_DURATION_LONG = 0.2f;

        private const string STICK_ANIMATION_IMAGE_BASE_PATH = "Images/Macros/stick_";

        public bool AppliesTo(IMacroInstruction macroInstruction) => macroInstruction is StickRotationMacroInstruction;

        public MacroInstructionTemplateViewModel CreateFor(IMacroInstruction macroInstruction)
        {
            if (macroInstruction is not StickRotationMacroInstruction stickRotationMacroInstruction)
                throw new ArgumentException($"The given macro instruction must be of type {nameof(StickRotationMacroInstruction)}.", nameof(macroInstruction));
        
            if (stickRotationMacroInstruction.Stick == Core.CommandModel.ControllerStick.Left)
            {
                return new MacroInstructionTemplateViewModel(
                        BuildFullCircleAnimation(
                            STICK_ANIMATION_IMAGE_BASE_PATH,
                            "L",
                            STICK_ROTATION_FRAME_DURATION_LONG,
                            STICK_ROTATION_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Animation,
                        static opt => new StickRotationMacroInstruction(
                            (opt as StickRotationMacroOptionsViewModel)?.SelectedStartDirection?.ControllerStickDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            (opt as StickRotationMacroOptionsViewModel)?.SelectedEndDirection?.ControllerStickDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            Core.CommandModel.ControllerStick.Left
                        )
                        {
                            IsCounterClockwise = (opt as StickRotationMacroOptionsViewModel)?.IsCounterClockwise ?? false
                        },
                        "Images/Macros/stick_L/Joy_L_PreView.png",
                        static mi => new StickRotationMacroOptionsViewModel(
                            (mi as StickRotationMacroInstruction)?.StartDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            (mi as StickRotationMacroInstruction)?.EndDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            (mi as StickRotationMacroInstruction)?.IsCounterClockwise ?? false
                        )
                    )
                { ToolTip = "Left Stick Rotation" };
            }
            else
            {
                return new MacroInstructionTemplateViewModel(
                        BuildFullCircleAnimation(
                            STICK_ANIMATION_IMAGE_BASE_PATH,
                            "R",
                            STICK_ROTATION_FRAME_DURATION_LONG,
                            STICK_ROTATION_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Animation,
                        static opt => new StickRotationMacroInstruction(
                            (opt as StickRotationMacroOptionsViewModel)?.SelectedStartDirection?.ControllerStickDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            (opt as StickRotationMacroOptionsViewModel)?.SelectedEndDirection?.ControllerStickDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            Core.CommandModel.ControllerStick.Right
                        )
                        {
                            IsCounterClockwise = (opt as StickRotationMacroOptionsViewModel)?.IsCounterClockwise ?? false
                        },
                        "Images/Macros/stick_R/Joy_R_PreView.png",
                        static mi => new StickRotationMacroOptionsViewModel(
                            (mi as StickRotationMacroInstruction)?.StartDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            (mi as StickRotationMacroInstruction)?.EndDirection ?? Core.CommandModel.ControllerStickDirection.North,
                            (mi as StickRotationMacroInstruction)?.IsCounterClockwise ?? false
                        ))
                { ToolTip = "Right Stick Rotation" };
            }
        }

        private static IEnumerable<AnimationFrame> BuildFullCircleAnimation(string stickImagePath, string stickName, float longFrameDuration, float shortFrameDuration)
        {
            for (int i = 0; i < 30; i++)
            {
                int frameIndex = i < 3 ? i : (3 + (i - 3) * 3);

                if (i >= 27)
                {
                    frameIndex = 3 - (i - 27);
                }

                float duration = i <= 0 ? longFrameDuration : shortFrameDuration;
                yield return new AnimationFrame($"{stickImagePath}{stickName}/Joy_{stickName}{frameIndex.ToString("00")}.{ANIMATION_FRAME_IMAGE_FILE_EXTENSION}", duration);
            }
        }
    }
}
