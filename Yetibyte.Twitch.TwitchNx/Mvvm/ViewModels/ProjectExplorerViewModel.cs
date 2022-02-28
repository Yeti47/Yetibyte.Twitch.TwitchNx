﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;
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
            private readonly IDocumentManager _documentManager;
            private readonly CommandSetup _commandSetup;
            private readonly RelayCommand _openCommand;

            private string _name = string.Empty;

            public string Name
            {
                get { return _name; }
                set { _name = value; OnPropertyChanged(); }
            }

            public ICommand OpenCommand => _openCommand;

            public CommandSetup CommandSetup => _commandSetup;

            public CommandViewModel(IDocumentManager documentManager, CommandSetup commandSetup)
            {
                _documentManager = documentManager;
                _commandSetup = commandSetup;

                _name = commandSetup.Name;

                _openCommand = new RelayCommand(() =>
                {
                    if (!_documentManager.IsDocumentOpen(typeof(CommandSetup), _commandSetup.Name))
                    {
                        _documentManager.OpenDocument(new CommandSetupDocumentViewModel(_documentManager, _commandSetup));
                    }
                    _documentManager.TrySelect(typeof(CommandSetup), _commandSetup.Name);
                });
            }
        }

        private readonly ObservableCollection<CommandViewModel> _commands = new ObservableCollection<CommandViewModel>();
        private readonly ObservableCollection<CooldownGroupViewModel> _cooldownGroups = new ObservableCollection<CooldownGroupViewModel>();
        private readonly RelayCommand _newCommandSetupCommand;
        
        private readonly IProjectManager _projectManager;
        private readonly IDocumentManager _documentManager;
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

        public ProjectExplorerViewModel(IProjectManager projectManager, IDocumentManager documentManager) : base("Project Explorer")
        {
            _projectManager = projectManager;
            _documentManager = documentManager;
            _projectManager.ProjectChanging += ProjectManager_ProjectChanging;
            _projectManager.ProjectChanged += ProjectManager_ProjectChanged;

            _newCommandSetupCommand = new RelayCommand(ExecuteNewCommandSetupCommand, CanExecuteNewCommandSetupCommand);

            _commands.CollectionChanged += commands_CollectionChanged;
            
        }

        private void commands_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems is not null)
            {
                foreach(CommandViewModel oldItem in e.OldItems)
                {
                    oldItem.CommandSetup.NameChanged -= CommandSetup_NameChanged;
                }
            }

            if (e.NewItems is not null)
            {
                foreach (CommandViewModel newItem in e.NewItems)
                {
                    newItem.CommandSetup.NameChanged += CommandSetup_NameChanged;
                }
            }
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
            _commands.Add(new CommandViewModel(_documentManager, e.CommandSetup));

        }

        private void CommandSetup_NameChanged(object? sender, NameChangedEventArgs e)
        {
            var commandVm = _commands.FirstOrDefault(c => c.Name == e.OldName);

            if (commandVm != null)
                commandVm.Name = e.NewName;
        }

        private void PopulateExplorerItems()
        {
            if (!_projectManager.IsProjectOpen || _projectManager.CurrentProject is null)
                return;

            _selectedCommand = null;
            _selectedCooldownGroup = null;

            _commands.Clear(); 
            _cooldownGroups.Clear(); 

            foreach(var commandVm in _projectManager.CurrentProject.CommandSettings.CommandSetups.Select(cs => new CommandViewModel(_documentManager, cs)))
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