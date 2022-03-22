using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;

namespace Yetibyte.Twitch.TwitchNx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const float BUTTON_FRAME_DURATION_SHORT = 0.08f;
        private const float BUTTON_FRAME_DURATION_LONG = 0.4f;

        private const string SIMPLE_BUTTON_IMAGE_BASE_PATH = "Images/Macros/";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ISwitchBridgeClientFactory switchBridgeClientFactory = new WebSocketSwitchBridgeClientFactory();
            SwitchConnector switchConnector = new SwitchConnector(switchBridgeClientFactory);

            IProjectManager projectManager = new ProjectManager();

            var mainWindow = new MainWindow(projectManager, switchConnector, static () =>
            {
                List<MacroInstructionTemplateViewModel> macroInstructionTemplates = new List<MacroInstructionTemplateViewModel>
                {
                    new MacroInstructionTemplateViewModel(
                        MacroInstructionTemplateViewModel.BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH, 
                            "A", 
                            BUTTON_FRAME_DURATION_LONG, 
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.A)
                    ),
                    new MacroInstructionTemplateViewModel(
                        MacroInstructionTemplateViewModel.BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "B",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.B)
                    ),
                    new MacroInstructionTemplateViewModel(
                        MacroInstructionTemplateViewModel.BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "X",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.X)
                    ),
                    new MacroInstructionTemplateViewModel(
                        MacroInstructionTemplateViewModel.BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Y",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Y)
                    ),
                    new MacroInstructionTemplateViewModel(
                        MacroInstructionTemplateViewModel.BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Minus",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Minus)
                    ),
                    new MacroInstructionTemplateViewModel(
                        MacroInstructionTemplateViewModel.BuildSimpleButtonAnimation(
                            SIMPLE_BUTTON_IMAGE_BASE_PATH,
                            "Plus",
                            BUTTON_FRAME_DURATION_LONG,
                            BUTTON_FRAME_DURATION_SHORT
                        ),
                        MacroInstructionType.Simple,
                        () => new ButtonPressMacroInstruction(ControllerButton.Plus)
                    ),
                    //// --------------------------------------
                    new MacroInstructionTemplateViewModel(
                        MacroInstructionTemplateViewModel.BuildFullCircleAnimation(
                            "Images/Macros/stick_",
                            "R",
                            0.2f,
                            0.04f
                        ),
                        MacroInstructionType.Animation,
                        () => new StickRotationMacroInstruction(
                            Core.CommandModel.ControllerStickDirection.North, 
                            Core.CommandModel.ControllerStickDirection.North, 
                            Core.CommandModel.ControllerStick.Right
                        ),
                        "Images/Macros/stick_R/Joy_R_PreView.png"
                    )
                };

                return macroInstructionTemplates;
            });

            this.MainWindow = mainWindow;
            this.MainWindow.Show();

        }
    }
}
