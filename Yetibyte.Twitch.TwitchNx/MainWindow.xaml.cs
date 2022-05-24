using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.Logging;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;
using Yetibyte.Twitch.TwitchNx.Mvvm.Views;
using Yetibyte.Twitch.TwitchNx.Services;
using Yetibyte.Twitch.TwitchNx.Services.Dialog;

namespace Yetibyte.Twitch.TwitchNx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string PROJECT_FILE_FILTER = $"TwitchNX Projects|*{ProjectManager.PROJECT_FILE_EXTENSION}";

        private readonly IProjectManager _projectManager;

        public MainViewModel ViewModel { get; private set; }

        public MainWindow(
            IProjectManager projectManager, 
            SwitchConnector switchConnector, 
            IMacroInstructionTemplateProvider macroInstructionTemplateProvider, 
            IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade, 
            IDialogService dialogService, 
            ICommandSourceProvider commandSourceProvider,
            EventLogAppender eventLogAppender)
        {
            InitializeComponent();
            
            DataContext = ViewModel = new MainViewModel(
                macroInstructionTemplateFactoryFacade, 
                projectManager, 
                switchConnector, 
                macroInstructionTemplateProvider, 
                dialogService, 
                commandSourceProvider,
                eventLogAppender
            );

            dialogService.MainContext = ViewModel;
            
            _projectManager = projectManager;
        }

        private void menuNewProject_Click(object sender, RoutedEventArgs e)
        {
            var customDialog = new CustomDialog(this)
            {
                Title = "New TwitchNx Project"
            };

            var dialogViewModel = new NewProjectViewModel(
               vm =>
               {
                   _projectManager.OpenNewProject(vm.ProjectName);
                   this.HideMetroDialogAsync(customDialog);
               },
               _ => this.HideMetroDialogAsync(customDialog)
            );

            customDialog.DataContext = dialogViewModel;
            customDialog.Content = new NewProjectView
            {
                DataContext = dialogViewModel
            };

            this.ShowMetroDialogAsync(customDialog);
        }

        private void menuLoadProject_Click(object sender, RoutedEventArgs e)
        {
            using System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = PROJECT_FILE_FILTER
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                _projectManager.OpenProject(openFileDialog.FileName);
            }
        }

        private void menuSaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (!_projectManager.IsProjectOpen)
                return;

            if (!_projectManager.IsFilePersisted)
            {
                SaveProjectAs();
            }
            else
            {
                _projectManager.SaveProject();
            }
        }

        private void menuSaveProjectAs_Click(object sender, RoutedEventArgs e)
        {
            SaveProjectAs();
        }

        private void SaveProjectAs()
        {
            if (!_projectManager.IsProjectOpen)
                return;

            using System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = PROJECT_FILE_FILTER
            };

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                _projectManager.ProjectFilePath = saveFileDialog.FileName;
                _projectManager.SaveProject();
            }
        }

    }
}
