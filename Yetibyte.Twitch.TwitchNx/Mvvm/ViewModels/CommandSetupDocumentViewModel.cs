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
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.MacroTimeLine;
using Yetibyte.Twitch.TwitchNx.Services;
using Yetibyte.Twitch.TwitchNx.Services.Dialog;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class CommandSetupDocumentViewModel : DocumentViewModel
    {
        public class CooldownGroupItem : ObservableObject
        {
            private string _name;

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }

            public bool IsDummy { get; }

            public CooldownGroupItem(string name, bool isDummy = false)
            {
                _name = name;
                IsDummy = isDummy;
            }
        }

        private static readonly List<PermissionLevel> _permissionLevels = new List<PermissionLevel> {
            PermissionLevel.Any, 
            PermissionLevel.Sub,
            PermissionLevel.Mod,
            PermissionLevel.Own
        };

        private readonly ObservableCollection<CooldownGroupItem> _cooldownGroupItems = new ObservableCollection<CooldownGroupItem>();

        private readonly IMacroTimeLineViewModelFactory _macroTimeLineViewModelFactory;
        private readonly IProjectManager _projectManager;
        private readonly CooldownGroupItem _dummyCooldownGroupItem;
        private readonly CommandSetup _commandSetup;
        private readonly RelayCommand _applyCommand;

        private string _commandName;
        private string _description;
        private bool _isDirty;
        private PermissionLevel _permissionLevel;
        private CooldownGroupItem _selectedCooldownGroupItem;

        public string CommandName
        {
            get { return _commandName; }
            set {
                _commandName = value;
                OnPropertyChanged();

                IsDirty = true;

                Validate();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();

                IsDirty = true;

                Validate();
            }
        }

        public IEnumerable<PermissionLevel> PermissionLevels => _permissionLevels;

        public PermissionLevel SelectedPermissionLevel
        {
            get => _permissionLevel;
            set
            {
                _permissionLevel = value;
                OnPropertyChanged();
                IsDirty = true;
            }
        }

        public IEnumerable<CooldownGroupItem> CooldownGroupItems => _cooldownGroupItems;

        public CooldownGroupItem SelectedCooldownGroupItem
        {
            get => _selectedCooldownGroupItem;
            set
            {
                _selectedCooldownGroupItem = value;
                OnPropertyChanged();
                IsDirty = true;
            }
        }

        public bool IsValid { get; private set; }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;

                OnPropertyChanged();

                Title = _commandSetup.Name + (_isDirty ? "*" : string.Empty);
            }
        }

        public override string DocumentName => _commandSetup.Name;

        public override Type DocumentType => _commandSetup.GetType();

        public ICommand ApplyCommand => _applyCommand;

        public MacroTimeLineViewModel MacroTimeLineViewModel { get; private set; }

        public CommandSetupDocumentViewModel(IDocumentManager documentManager, CommandSetup commandSetup, IMacroTimeLineViewModelFactory macroTimeLineViewModelFactory, IProjectManager projectManager) : base(documentManager)
        {
            _commandSetup = commandSetup;
            _commandName = commandSetup.Name;
            _description = commandSetup.Description;
            _permissionLevel = commandSetup.PermissionLevel;
            _macroTimeLineViewModelFactory = macroTimeLineViewModelFactory;
            _projectManager = projectManager;

            _dummyCooldownGroupItem = new CooldownGroupItem(string.Empty, true);
            _selectedCooldownGroupItem = _dummyCooldownGroupItem;

            MacroTimeLineViewModel = _macroTimeLineViewModelFactory.CreateViewModel(commandSetup.Macro);

            Title = _commandName;

            _projectManager.ProjectChanging += _projectManager_ProjectChanging;
            _projectManager.ProjectChanged += _projectManager_ProjectChanged;

            if (_projectManager.CurrentProject is not null)
            {
                _projectManager.CurrentProject.CommandSettings.CooldownGroupAdded += CommandSettings_CooldownGroupAdded;
                _projectManager.CurrentProject.CommandSettings.CooldownGroupRemoved += CommandSettings_CooldownGroupRemoved;
            }

            PopulateCooldownGroupItems();

            _applyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);

            Validate();

            IsDirty = false;
        }

        private void _projectManager_ProjectChanging(object? sender, EventArgs e)
        {
            if (_projectManager.CurrentProject is null)
                return;

            _projectManager.CurrentProject.CommandSettings.CooldownGroupAdded -= CommandSettings_CooldownGroupAdded;
            _projectManager.CurrentProject.CommandSettings.CooldownGroupRemoved -= CommandSettings_CooldownGroupRemoved;
        }

        private void _projectManager_ProjectChanged(object? sender, EventArgs e)
        {
            if (_projectManager.CurrentProject is null)
                return;

            _projectManager.CurrentProject.CommandSettings.CooldownGroupAdded += CommandSettings_CooldownGroupAdded;
            _projectManager.CurrentProject.CommandSettings.CooldownGroupRemoved += CommandSettings_CooldownGroupRemoved;

            PopulateCooldownGroupItems();
        }

        private void PopulateCooldownGroupItems()
        {
            if (_projectManager.CurrentProject is null)
                return;

            _cooldownGroupItems.Clear();

            _cooldownGroupItems.Add(_dummyCooldownGroupItem);

            foreach (var cooldownGroup in _projectManager.CurrentProject.CommandSettings.CooldownGroups)
            {
                CooldownGroupItem cooldownGroupItem = new CooldownGroupItem(cooldownGroup.Name);

                _cooldownGroupItems.Add(cooldownGroupItem);

                cooldownGroup.NameChanged += CooldownGroup_NameChanged;
            }

            SelectedCooldownGroupItem = _cooldownGroupItems
                .FirstOrDefault(c => c.Name.Equals(_commandSetup?.CooldownGroup?.Name, StringComparison.OrdinalIgnoreCase)) 
                ?? _dummyCooldownGroupItem;
        }

        private void CommandSettings_CooldownGroupRemoved(object? sender, CooldownGroupRemovedEventArgs e)
        {
            if (_cooldownGroupItems.FirstOrDefault(c => c.Name.Equals(e.CooldownGroup.Name, StringComparison.OrdinalIgnoreCase)) is not CooldownGroupItem cooldownGroupItem || cooldownGroupItem.IsDummy)
                return;

            e.CooldownGroup.NameChanged -= CooldownGroup_NameChanged;

            _cooldownGroupItems.Remove(cooldownGroupItem);

            if (SelectedCooldownGroupItem == cooldownGroupItem)
                SelectedCooldownGroupItem = _dummyCooldownGroupItem;
        }

        private void CommandSettings_CooldownGroupAdded(object? sender, CooldownGroupAddedEventArgs e)
        {
            CooldownGroupItem cooldownGroupItem = new CooldownGroupItem(e.CooldownGroup.Name);

            e.CooldownGroup.NameChanged += CooldownGroup_NameChanged;

            if (!_cooldownGroupItems.Any(c => c.Name.Equals(cooldownGroupItem.Name, StringComparison.OrdinalIgnoreCase)))
                _cooldownGroupItems.Add(cooldownGroupItem);

        }

        private void CooldownGroup_NameChanged(object? sender, Core.Common.NameChangedEventArgs e)
        {
            if (_cooldownGroupItems.FirstOrDefault(c => c.Name.Equals(e.OldName, StringComparison.OrdinalIgnoreCase)) is not CooldownGroupItem cooldownGroupItem || cooldownGroupItem.IsDummy)
                return;

            cooldownGroupItem.Name = e.NewName;
        }

        private bool CanExecuteApplyCommand() => IsValid;

        private void ExecuteApplyCommand()
        {
            if (!IsActive)
                return;

            CommandName = CommandName.Trim();
            Description = Description.Trim();

            _commandSetup.Name = _commandName.Trim();
            _commandSetup.Description = _description.Trim();
            _commandSetup.PermissionLevel = _permissionLevel;

            _commandSetup.CooldownGroup = _projectManager.CurrentProject?.CommandSettings?.CooldownGroups.FirstOrDefault(cdg => cdg.Name.Equals(SelectedCooldownGroupItem.Name, StringComparison.OrdinalIgnoreCase));   

            MacroTimeLineViewModel.ApplyChanges();

            IsDirty = false;

        }

        public bool Validate()
        {
            IsValid = !string.IsNullOrWhiteSpace(CommandName);

            if (_projectManager.CurrentProject is not null)
                IsValid &= !_projectManager.CurrentProject.CommandSettings.CommandSetups.Any(c => c != _commandSetup && c.Name.Equals(CommandName, StringComparison.OrdinalIgnoreCase));

            _applyCommand.NotifyCanExecuteChanged();

            return IsValid;
        }

    }
}
