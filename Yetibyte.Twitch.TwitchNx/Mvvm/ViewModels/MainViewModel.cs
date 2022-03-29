using AvalonDock.Themes;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Services;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IProjectManager _projectManager;
        private readonly SwitchConnector _switchConnector;
        private readonly ObservableCollection<ToolViewModel> _tools;

        private SwitchConnectionViewModel _switchConnectionViewModel;

        private readonly RelayCommand _closeProjectCommand;

        private bool _isProjectOpen;

        public ICommand CloseProjectCommand => _closeProjectCommand;

        public SwitchConnectionViewModel SwitchConnectionViewModel
        {
            get => _switchConnectionViewModel;
            private set
            {
                _switchConnectionViewModel = value;
                OnPropertyChanged();
            }
        }

        public SwitchControlViewModel SwitchControlViewModel { get; private set; }
        public ProjectExplorerViewModel ProjectExplorerViewModel { get; private set; }

        public DocumentManagerViewModel DocumentManagerViewModel { get; private set; }
        public MacroTesterViewModel MacroTesterViewModel { get; private set; }
        public MacroToolBoxViewModel MacroToolBoxViewModel { get; private set; }

        public IEnumerable<ToolViewModel> Tools => _tools;

        public Theme Theme { get; } = new AvalonDock.Themes.Vs2013LightTheme();

        public bool CanEditProject
        {
            get => _isProjectOpen;
            set
            {
                _isProjectOpen = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade, IProjectManager projectManager, SwitchConnector switchConnector, IMacroInstructionTemplateProvider macroInstructionTemplateProvider)
        {
            _projectManager = projectManager;
            _switchConnector = switchConnector;

            _switchConnectionViewModel = new SwitchConnectionViewModel(switchConnector, projectManager.CurrentProject?.SwitchBridgeClientConnectionSettings ?? SwitchBridgeClientConnectionSettings.CreateEmpty());

            DocumentManagerViewModel = new DocumentManagerViewModel();
            SwitchControlViewModel = new SwitchControlViewModel(switchConnector);
            ProjectExplorerViewModel = new ProjectExplorerViewModel(macroInstructionTemplateFactoryFacade, projectManager, DocumentManagerViewModel);
            MacroTesterViewModel = new MacroTesterViewModel(switchConnector, SwitchControlViewModel);

            IEnumerable<MacroInstructionTemplateViewModel> macroInstructionTemplateViewModels = macroInstructionTemplateProvider.GetMacroInstructionTemplates();

            MacroToolBoxViewModel = new MacroToolBoxViewModel(macroInstructionTemplateViewModels);

            _projectManager.ProjectChanged += projectManager_ProjectChanged;
            _projectManager.ProjectChanging += _projectManager_ProjectChanging;

            _tools = new ObservableCollection<ToolViewModel> { 
                SwitchConnectionViewModel, 
                SwitchControlViewModel, 
                ProjectExplorerViewModel, 
                MacroTesterViewModel,
                MacroToolBoxViewModel
            };

            _closeProjectCommand = new RelayCommand(() => _projectManager.CloseProject(), () => _projectManager.IsProjectOpen);
        }

        private void _projectManager_ProjectChanging(object? sender, EventArgs e)
        {

        }

        private void projectManager_ProjectChanged(object? sender, EventArgs e)
        {
            CanEditProject = _projectManager.IsProjectOpen;

            SwitchConnectionViewModel.SwitchBridgeClientConnectionSettings = _projectManager.CurrentProject?.SwitchBridgeClientConnectionSettings ?? SwitchBridgeClientConnectionSettings.CreateEmpty();

            _closeProjectCommand.NotifyCanExecuteChanged();

        }

    }
}
