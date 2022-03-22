using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using static Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroInstructionTemplateViewModel;

namespace Yetibyte.Twitch.TwitchNx.Util
{
    internal static class MacroInstructionTemplateHelper
    {
        public const string ANIMATION_FRAME_IMAGE_FILE_EXTENSION = "png";

        private const float BUTTON_FRAME_DURATION_SHORT = 0.08f;
        private const float BUTTON_FRAME_DURATION_LONG = 0.4f;

        private const float STICK_ROTATION_FRAME_DURATION_SHORT = 0.04f;
        private const float STICK_ROTATION_FRAME_DURATION_LONG = 0.2f;

        private const string SIMPLE_BUTTON_IMAGE_BASE_PATH = "Images/Macros/";
        private const string STICK_ANIMATION_IMAGE_BASE_PATH = "Images/Macros/stick_";

        public static IEnumerable<AnimationFrame> BuildSimpleButtonAnimation(string basePath, string buttonName, float longDuration, float shortDuration)
        {
            for (int i = 1; i <= 4; i++)
            {
                yield return new AnimationFrame($"{basePath}btn_{buttonName}/{buttonName}_{i}.{ANIMATION_FRAME_IMAGE_FILE_EXTENSION}", i <= 1 ? longDuration : shortDuration);
            }
        }

        public static IEnumerable<AnimationFrame> BuildFullCircleAnimation(string stickImagePath, string stickName, float longFrameDuration, float shortFrameDuration)
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

        public static IEnumerable<MacroInstructionTemplateViewModel> CreateMacroInstructionTemplateViewModels()
        {
            List<MacroInstructionTemplateViewModel> macroInstructionTemplates = new List<MacroInstructionTemplateViewModel>
                {
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "A",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.A)
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "B",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.B)
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "X",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.X)
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Y",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Y)
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Plus",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Plus)
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Minus",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Minus)
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Home",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Home)
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Capture",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Capture)
                    ),
                    //// --------------------------------------
                    new MacroInstructionTemplateViewModel(
                        BuildFullCircleAnimation(
                            STICK_ANIMATION_IMAGE_BASE_PATH,
                            "L",
                            STICK_ROTATION_FRAME_DURATION_LONG,
                            STICK_ROTATION_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Animation,
                        () => new StickRotationMacroInstruction(
                            Core.CommandModel.ControllerStickDirection.North,
                            Core.CommandModel.ControllerStickDirection.North,
                            Core.CommandModel.ControllerStick.Left
                        ),
                        "Images/Macros/stick_L/Joy_L_PreView.png"
                    ),
                    new MacroInstructionTemplateViewModel(
                        BuildFullCircleAnimation(
                            STICK_ANIMATION_IMAGE_BASE_PATH,
                            "R",
                            STICK_ROTATION_FRAME_DURATION_LONG,
                            STICK_ROTATION_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Animation,
                        () => new StickRotationMacroInstruction(
                            Core.CommandModel.ControllerStickDirection.North,
                            Core.CommandModel.ControllerStickDirection.North,
                            Core.CommandModel.ControllerStick.Right
                        ),
                        "Images/Macros/stick_R/Joy_R_PreView.png"
                    ),
                };

            return macroInstructionTemplates;
        }
    }
}
