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
using Yetibyte.Twitch.TwitchNx.Services;
using Yetibyte.Twitch.TwitchNx.Services.Dialog;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.Irc;

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

            IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade = new MacroInstructionTemplateFactoryFacade();

            IMacroInstructionTemplateProvider macroInstructionTemplateProvider = new DefaultMacroInstructionTemplateProvider(macroInstructionTemplateFactoryFacade);

            IDialogService dialogService = new DialogService();

            ICommandSourceProvider commandSourceProvider = CreateCommandSourceProvider();

            var mainWindow = new MainWindow(projectManager, switchConnector, macroInstructionTemplateProvider, macroInstructionTemplateFactoryFacade, dialogService, commandSourceProvider);

            this.MainWindow = mainWindow;
            this.MainWindow.Show();

        }

        private ICommandSourceProvider CreateCommandSourceProvider()
        {
            IrcCommandSourceFactory ircCommandSourceFactory = new IrcCommandSourceFactory("Twitch IRC", new TwitchIrcClient());

            CommandSourceProvider commandSourceProvider = new CommandSourceProvider(new []
            {
                ircCommandSourceFactory
            });

            return commandSourceProvider;
        }

        
    }
}
