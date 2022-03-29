using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using static Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroInstructionTemplateViewModel;

namespace Yetibyte.Twitch.TwitchNx.Services.MacroInstructionTemplateFactories
{
    public class ButtonPressMacroInstructionTemplateFactory : IMacroInstructionTemplateFactory
    {
        private const string ANIMATION_FRAME_IMAGE_FILE_EXTENSION = "png";

        private const float BUTTON_FRAME_DURATION_SHORT = 0.08f;
        private const float BUTTON_FRAME_DURATION_LONG = 0.4f;

        private const float DPAD_FRAME_DURATION_LONG = 0.6f;
        private const float DPAD_FRAME_DURATION_SHORT = 0.5f;

        private const float LR_FRAME_DURATION_LONG = 0.6f;
        private const float LR_FRAME_DURATION_SHORT = 0.5f;

        private const string SIMPLE_BUTTON_IMAGE_BASE_PATH = "Images/Macros/";

        public bool AppliesTo(IMacroInstruction macroInstruction) => macroInstruction is ButtonPressMacroInstruction buttonPressMacroInstruction;

        public MacroInstructionTemplateViewModel CreateFor(IMacroInstruction macroInstruction)
        {
            if (macroInstruction is not ButtonPressMacroInstruction buttonPressMacroInstruction)
                throw new ArgumentException($"The given macro instruction must be of type {nameof(ButtonPressMacroInstruction)}.", nameof(macroInstruction));

            if (buttonPressMacroInstruction.Button.IsDpad)
                return CreateDpadTemplate(buttonPressMacroInstruction);
            else if (buttonPressMacroInstruction.Button.IsTrigger)
                return CreateTriggerTemplate(buttonPressMacroInstruction);
            else
                return CreateButtonPressTemplate(buttonPressMacroInstruction);
        }

        private MacroInstructionTemplateViewModel CreateTriggerTemplate(ButtonPressMacroInstruction buttonPressMacroInstruction)
        {
            MacroInstructionTemplateViewModel macroInstructionTemplateViewModel;

            if (buttonPressMacroInstruction.Button == ControllerButton.L)
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/btn_L/L_1.png", LR_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/btn_L/L_2.png", LR_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.L)
                    )
                { ToolTip = "Press L Button" };
            }
            else if (buttonPressMacroInstruction.Button == ControllerButton.R)
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/btn_R/R_1.png", LR_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/btn_R/R_2.png", LR_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.R)
                    )
                { ToolTip = "Press R Button" };
            }
            else if (buttonPressMacroInstruction.Button == ControllerButton.ZL)
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/btn_ZL/ZL_1.png", LR_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/btn_ZL/ZL_2.png", LR_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.ZL)
                    )
                { ToolTip = "Press ZL Trigger" };
            }
            else
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/btn_ZR/ZR_1.png", LR_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/btn_ZR/ZR_2.png", LR_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.ZR)
                    )
                { ToolTip = "Press ZR Trigger" };
            }

            return macroInstructionTemplateViewModel;
        }

        private MacroInstructionTemplateViewModel CreateDpadTemplate(ButtonPressMacroInstruction buttonPressMacroInstruction)
        {
            MacroInstructionTemplateViewModel macroInstructionTemplateViewModel;

            if (buttonPressMacroInstruction.Button == ControllerButton.DpadUp)
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/dpad/Dpad_1.png", DPAD_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/dpad/Dpad_2.png", DPAD_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.DpadUp),
                        "Images/Macros/dpad/Dpad_2.png"
                    )
                { ToolTip = "Press D-Pad Up" };
            }
            else if (buttonPressMacroInstruction.Button == ControllerButton.DpadRight)
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/dpad/Dpad_1.png", DPAD_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/dpad/Dpad_3.png", DPAD_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.DpadRight),
                        "Images/Macros/dpad/Dpad_3.png"
                    )
                { ToolTip = "Press D-Pad Right" };
            }
            else if (buttonPressMacroInstruction.Button == ControllerButton.DpadDown)
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/dpad/Dpad_1.png", DPAD_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/dpad/Dpad_4.png", DPAD_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.DpadDown),
                        "Images/Macros/dpad/Dpad_4.png"
                    )
                { ToolTip = "Press D-Pad Down" };
            }
            else
            {
                macroInstructionTemplateViewModel = new MacroInstructionTemplateViewModel(
                        new[] {
                            new AnimationFrame("Images/Macros/dpad/Dpad_1.png", DPAD_FRAME_DURATION_LONG),
                            new AnimationFrame("Images/Macros/dpad/Dpad_5.png", DPAD_FRAME_DURATION_SHORT),
                        },
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.DpadLeft),
                        "Images/Macros/dpad/Dpad_5.png"
                    )
                { ToolTip = "Press D-Pad Left" };
            }

            return macroInstructionTemplateViewModel;
        }

        private static MacroInstructionTemplateViewModel CreateButtonPressTemplate(ButtonPressMacroInstruction buttonPressMacroInstruction)
        {
            return new MacroInstructionTemplateViewModel(
                BuildSimpleButtonAnimation(
                    SIMPLE_BUTTON_IMAGE_BASE_PATH,
                    buttonPressMacroInstruction.Button?.Macro ?? ControllerButton.A.Macro,
                    BUTTON_FRAME_DURATION_LONG,
                    BUTTON_FRAME_DURATION_SHORT
                ),
                MacroInstructionType.Simple,
                () => new ButtonPressMacroInstruction(buttonPressMacroInstruction.Button ?? ControllerButton.A)
            )
            {
                ToolTip = $"Press {buttonPressMacroInstruction.Button?.Name}"
            };
        }

        private static IEnumerable<AnimationFrame> BuildSimpleButtonAnimation(string basePath, string buttonName, float longDuration, float shortDuration)
        {
            for (int i = 1; i <= 4; i++)
            {
                yield return new AnimationFrame($"{basePath}btn_{buttonName}/{buttonName}_{i}.{ANIMATION_FRAME_IMAGE_FILE_EXTENSION}", i <= 1 ? longDuration : shortDuration);
            }
        }
    }
}
