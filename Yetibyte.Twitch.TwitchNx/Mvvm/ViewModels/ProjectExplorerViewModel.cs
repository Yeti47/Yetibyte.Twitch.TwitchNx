using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Styling;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    [DefaultPane("MainRight")]
    public class ProjectExplorerViewModel : ToolViewModel
    {
        public class CooldownGroupViewModel : ObservableObject
        {
            private string _name = string.Empty;

            public string Name
            {
                get { return _name; }
                set { _name = value; OnPropertyChanged(); }
            }
        }

        public class CommandViewModel : ObservableObject
        {
            private string _name = string.Empty;

            public string Name
            {
                get { return _name; }
                set { _name = value; OnPropertyChanged(); }
            }
        }

        private readonly ObservableCollection<CommandViewModel> _commands = new ObservableCollection<CommandViewModel>();
        private readonly ObservableCollection<CooldownGroupViewModel> _cooldownGroups = new ObservableCollection<CooldownGroupViewModel>();
        private readonly RelayCommand _newCommandSetupCommand;
        
        private readonly IProjectManager _projectManager;


        private CommandViewModel? _selectedCommand;
        private CooldownGroupViewModel? _selectedCooldownGroup;

        public CommandViewModel? SelectedCommand
        {
            get { return _selectedCommand; }
            set { _selectedCommand = value; OnPropertyChanged(); }
        }

        public CooldownGroupViewModel? SelectedCooldownGroup
        {
            get { return _selectedCooldownGroup; }
            set { _selectedCooldownGroup = value; OnPropertyChanged(); }
        }

        public ICommand NewCommandSetupCommand => _newCommandSetupCommand;

        public IEnumerable<CommandViewModel> Commands => _commands;
        public IEnumerable<CooldownGroupViewModel> CooldownGroups => _cooldownGroups;

        public ProjectExplorerViewModel(IProjectManager projectManager) : base("Project Explorer")
        {
            _projectManager = projectManager;
            _projectManager.ProjectChanging += ProjectManager_ProjectChanging;
            _projectManager.ProjectChanged += ProjectManager_ProjectChanged;

            _newCommandSetupCommand = new RelayCommand(ExecuteNewCommandSetupCommand, CanExecuteNewCommandSetupCommand);
            
        }

        private bool CanExecuteNewCommandSetupCommand()
        {
            return _projectManager.IsProjectOpen;
        }

        private void ExecuteNewCommandSetupCommand()
        {
            if (_projectManager.CurrentProject is null)
                return;

            string commandName = _projectManager.CurrentProject.CommandSettings.GetFreeDefaultCommandName();
            CommandSetup commandSetup = new CommandSetup(commandName);

            _projectManager.CurrentProject.CommandSettings.AddCommandSetup(commandSetup);

            SelectedCommand = _commands.FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));    
        }

        private void ProjectManager_ProjectChanging(object? sender, EventArgs e)
        {
            if (_projectManager.CurrentProject is not null)
            {
                _projectManager.CurrentProject.CommandSettings.CommandSetupAdded -= CommandSettings_CommandSetupAdded;
                _projectManager.CurrentProject.CommandSettings.CooldownGroupAdded -= CommandSettings_CooldownGroupAdded;

                _projectManager.CurrentProject.CommandSettings.CommandSetupRemoved -= CommandSettings_CommandSetupRemoved;
                _projectManager.CurrentProject.CommandSettings.CooldownGroupRemoved -= CommandSettings_CooldownGroupRemoved;
            }
        }

        private void ProjectManager_ProjectChanged(object? sender, EventArgs e)
        {
            if (_projectManager.CurrentProject is not null)
            {
                _projectManager.CurrentProject.CommandSettings.CommandSetupAdded += CommandSettings_CommandSetupAdded;
                _projectManager.CurrentProject.CommandSettings.CooldownGroupAdded += CommandSettings_CooldownGroupAdded;

                _projectManager.CurrentProject.CommandSettings.CommandSetupRemoved += CommandSettings_CommandSetupRemoved;
                _projectManager.CurrentProject.CommandSettings.CooldownGroupRemoved += CommandSettings_CooldownGroupRemoved;
            }

            _newCommandSetupCommand.NotifyCanExecuteChanged();
            
            PopulateExplorerItems();
        }

        private void CommandSettings_CooldownGroupRemoved(object? sender, Core.CommandModel.CooldownGroupRemovedEventArgs e)
        {
            var cooldownGroupVm = _cooldownGroups.FirstOrDefault(cg => cg.Name == e.CooldownGroup.Name);

            if (cooldownGroupVm != null)
                _cooldownGroups.Remove(cooldownGroupVm);
        }

        private void CommandSettings_CommandSetupRemoved(object? sender, Core.CommandModel.CommandSetupRemovedEventArgs e)
        {
            var commandVm = _commands.FirstOrDefault(c => c.Name == e.CommandSetup.Name);

            if (commandVm != null)
                _commands.Remove(commandVm);
        }

        private void CommandSettings_CooldownGroupAdded(object? sender, Core.CommandModel.CooldownGroupAddedEventArgs e)
        {
            _cooldownGroups.Add(new CooldownGroupViewModel { Name = e.CooldownGroup.Name });
        }

        private void CommandSettings_CommandSetupAdded(object? sender, Core.CommandModel.CommandSetupAddedEventArgs e)
        {
            _commands.Add(new CommandViewModel { Name = e.CommandSetup.Name });
        }

        private void PopulateExplorerItems()
        {
            if (!_projectManager.IsProjectOpen || _projectManager.CurrentProject is null)
                return;

            _selectedCommand = null;
            _selectedCooldownGroup = null;

            _commands.Clear(); 
            _cooldownGroups.Clear(); 

            foreach(var commandVm in _projectManager.CurrentProject.CommandSettings.CommandSetups.Select(cs => new CommandViewModel { Name = cs.Name}))
            {
                _commands.Add(commandVm);
            }

            foreach(var cooldownVm in _projectManager.CurrentProject.CommandSettings.CooldownGroups.Select(cg => new CooldownGroupViewModel { Name = cg.Name }))
            {
                _cooldownGroups.Add(cooldownVm);
            }
        }
    }
}
