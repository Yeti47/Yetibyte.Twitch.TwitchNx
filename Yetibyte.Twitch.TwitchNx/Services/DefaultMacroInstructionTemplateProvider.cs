using System.Collections.Generic;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    internal class DefaultMacroInstructionTemplateProvider : IMacroInstructionTemplateProvider
    {
        private static readonly IMacroInstruction[] TEMPLATE_INSTRUCTIONS = new IMacroInstruction[]
        {
            new ButtonPressMacroInstruction(ControllerButton.A),
            new ButtonPressMacroInstruction(ControllerButton.B),
            new ButtonPressMacroInstruction(ControllerButton.X),
            new ButtonPressMacroInstruction(ControllerButton.Y),
            new ButtonPressMacroInstruction(ControllerButton.Plus),
            new ButtonPressMacroInstruction(ControllerButton.Minus),
            new ButtonPressMacroInstruction(ControllerButton.Home),
            new ButtonPressMacroInstruction(ControllerButton.Capture),
            new ButtonPressMacroInstruction(ControllerButton.L),
            new ButtonPressMacroInstruction(ControllerButton.R),
            new ButtonPressMacroInstruction(ControllerButton.ZL),
            new ButtonPressMacroInstruction(ControllerButton.ZR),
            new ButtonPressMacroInstruction(ControllerButton.DpadUp),
            new ButtonPressMacroInstruction(ControllerButton.DpadRight),
            new ButtonPressMacroInstruction(ControllerButton.DpadDown),
            new ButtonPressMacroInstruction(ControllerButton.DpadLeft),
            new StickRotationMacroInstruction(ControllerStickDirection.North, ControllerStickDirection.North, ControllerStick.Left),
            new StickRotationMacroInstruction(ControllerStickDirection.North, ControllerStickDirection.North, ControllerStick.Right),
            new KeyFrameMacroInstruction()
        };

        private readonly IMacroInstructionTemplateFactoryFacade _macroInstructionTemplateFactoryFacade;

        public DefaultMacroInstructionTemplateProvider(IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade)
        {
            _macroInstructionTemplateFactoryFacade = macroInstructionTemplateFactoryFacade;
        }

        public IEnumerable<MacroInstructionTemplateViewModel> GetMacroInstructionTemplates()
        {
            foreach (IMacroInstruction macroInstruction in TEMPLATE_INSTRUCTIONS)
                yield return _macroInstructionTemplateFactoryFacade.CreateFor(macroInstruction);
        }
    }
}
