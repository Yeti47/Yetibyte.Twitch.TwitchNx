using AvalonDock.Themes;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IProjectManager _projectManager;
        private readonly SwitchConnector _switchConnector;
        private readonly ToolViewModel[] _tools;

        private readonly RelayCommand _closeProjectCommand;

        private bool _isProjectOpen;

        public ICommand CloseProjectCommand => _closeProjectCommand;

        public SwitchConnectionViewModel SwitchConnectionViewModel { get; private set; }
        public SwitchControlViewModel SwitchControlViewModel { get; private set; }

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

        public MainViewModel(IProjectManager projectManager, SwitchConnector switchConnector)
        {
            _projectManager = projectManager;
            _switchConnector = switchConnector;

            SwitchConnectionViewModel = new SwitchConnectionViewModel(switchConnector);
            SwitchConnectionViewModel.PropertyChanged += SwitchConnectionViewModel_PropertyChanged;

            SwitchControlViewModel = new SwitchControlViewModel(switchConnector);

            _projectManager.ProjectChanged += projectManager_ProjectChanged;
            _projectManager.ProjectChanging += _projectManager_ProjectChanging;

            _tools = new ToolViewModel[] { SwitchConnectionViewModel, SwitchControlViewModel };

            _closeProjectCommand = new RelayCommand(() => _projectManager.CloseProject(), () => _projectManager.IsProjectOpen);
        }

        private void _projectManager_ProjectChanging(object? sender, EventArgs e)
        {
            if (_projectManager.CurrentProject is not null)
            {
                _projectManager.CurrentProject.SwitchBridgeClientConnectionSettings.SettingsChanged -= SwitchBridgeClientConnectionSettings_SettingsChanged;
            }
        }

        private void projectManager_ProjectChanged(object? sender, EventArgs e)
        {
            UpdateConnectionViewModel();

            CanEditProject = _projectManager.IsProjectOpen;

            if (_projectManager.CurrentProject is not null)
            {
                _projectManager.CurrentProject.SwitchBridgeClientConnectionSettings.SettingsChanged += SwitchBridgeClientConnectionSettings_SettingsChanged;
            }

            _closeProjectCommand.NotifyCanExecuteChanged();

        }
        private void UpdateConnectionViewModel()
        {
            SwitchConnectionViewModel.ClientAddress = _projectManager.CurrentProject?.SwitchBridgeClientConnectionSettings?.Address ?? string.Empty;
            SwitchConnectionViewModel.ClientPort = _projectManager.CurrentProject?.SwitchBridgeClientConnectionSettings?.Port ?? SwitchBridgeClientConnectionSettings.DEFAULT_PORT;
        }

        private void SwitchBridgeClientConnectionSettings_SettingsChanged(object? sender, EventArgs e)
        {
            UpdateConnectionViewModel();
        }

        private void SwitchConnectionViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Project? project = _projectManager.CurrentProject;

            if (project is not null) 
            {
                if (e.PropertyName == nameof(SwitchConnectionViewModel.ClientAddress))
                    project.SwitchBridgeClientConnectionSettings.Address = SwitchConnectionViewModel.ClientAddress;

                if (e.PropertyName == nameof(SwitchConnectionViewModel.ClientPort))
                    project.SwitchBridgeClientConnectionSettings.Port = SwitchConnectionViewModel.ClientPort;
            }
        }
    }
}
