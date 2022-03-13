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
                        "Images/Macros/btn_A/",
                        0.1,
                        MacroInstructionType.Simple,
                        () => null
                    ),
                    new MacroInstructionTemplateViewModel(
                        "Images/Macros/btn_B/",
                        0.1,
                        MacroInstructionType.Simple,
                        () => null
                    )
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_A.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_B.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_X.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_Y.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_L.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_R.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_ZL.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_ZR.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_SL.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_SR.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_plus.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_minus.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_minus.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_home.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_capture.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_DPAD_UP.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_DPAD_DOWN.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_DPAD_LEFT.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/btn_DPAD_RIGHT.gif",
                    //    MacroInstructionType.Simple,
                    //    () => null
                    //),
                    //// --------------------------------------
                    //new MacroInstructionTemplateViewModel(
                    //    "/Images/Macros/stick_R.gif",
                    //    MacroInstructionType.Animation,
                    //    () => null
                    //),
                };

                return macroInstructionTemplates;
            });

            this.MainWindow = mainWindow;
            this.MainWindow.Show();

        }
    }
}
