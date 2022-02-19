using AvalonDock.Themes;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public SwitchConnectionViewModel SwitchConnectionViewModel { get; private set; }
        public SwitchControlViewModel SwitchControlViewModel { get; private set; }

        public IEnumerable<ToolViewModel> Tools => _tools;

        public Theme Theme { get; } = new AvalonDock.Themes.Vs2013LightTheme();

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

            if (_projectManager.CurrentProject is not null)
            {
                _projectManager.CurrentProject.SwitchBridgeClientConnectionSettings.SettingsChanged += SwitchBridgeClientConnectionSettings_SettingsChanged;
            }

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
                project.SwitchBridgeClientConnectionSettings.Address = SwitchConnectionViewModel.ClientAddress;
                project.SwitchBridgeClientConnectionSettings.Port = SwitchConnectionViewModel.ClientPort;
            }
        }
    }
}
