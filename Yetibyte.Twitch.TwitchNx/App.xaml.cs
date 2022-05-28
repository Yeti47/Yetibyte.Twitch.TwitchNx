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
using log4net;
using log4net.Config;
using Yetibyte.Twitch.TwitchNx.Core.Logging;
using log4net.Core;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.CommandSourceGui;

namespace Yetibyte.Twitch.TwitchNx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string LOG_FILE_PATH = "twitchnx.log";

        private static readonly ILog _logger = LogManager.GetLogger(ApplicationConstants.ROOT_LOGGER_NAME);
        private EventLogAppender _eventLogAppender = new EventLogAppender();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureLogging();

            ISwitchBridgeClientFactory switchBridgeClientFactory = new WebSocketSwitchBridgeClientFactory();
            SwitchConnector switchConnector = new SwitchConnector(switchBridgeClientFactory);

            ICommandSourceProvider commandSourceProvider = new DefaultCommandSourceProvider();
            commandSourceProvider.Load();

            CommandSourceSettingsViewService commandSourceSettingsViewService = new CommandSourceSettingsViewService();

            commandSourceSettingsViewService.RegisterCommandSourceSettingsDataTemplates();

            IProjectManager projectManager = new ProjectManager(commandSourceProvider, _logger);

            IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade = new MacroInstructionTemplateFactoryFacade();

            IMacroInstructionTemplateProvider macroInstructionTemplateProvider = new DefaultMacroInstructionTemplateProvider(macroInstructionTemplateFactoryFacade);

            IDialogService dialogService = new DialogService();

            var mainWindow = new MainWindow(
                projectManager, 
                switchConnector, 
                macroInstructionTemplateProvider, 
                macroInstructionTemplateFactoryFacade, 
                dialogService,
                commandSourceProvider,
                _eventLogAppender
            );

            this.MainWindow = mainWindow;
            this.MainWindow.Show();

            _logger.Info("Application started.");

        }

        private void ConfigureLogging()
        {
            var fileAppender = new log4net.Appender.FileAppender
            {
                AppendToFile = true,
                Threshold = log4net.Core.Level.Debug,
                Encoding = System.Text.Encoding.UTF8,
                LockingModel = new log4net.Appender.FileAppender.MinimalLock(),
                Name = "FileAppender",
                File = LOG_FILE_PATH,
                Layout = new log4net.Layout.PatternLayout("[%date{yyyy-MM-dd hh:mm:ss}] %level: %message%newline")
            };

            fileAppender.ActivateOptions();

            BasicConfigurator.Configure(
                fileAppender,
                _eventLogAppender
            );
        }

    }
}
