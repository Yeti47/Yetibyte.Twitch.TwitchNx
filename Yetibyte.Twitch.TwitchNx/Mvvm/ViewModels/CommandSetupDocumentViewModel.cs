using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
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
        private static readonly List<PermissionLevel> _permissionLevels = new List<PermissionLevel> {
            PermissionLevel.Any, 
            PermissionLevel.Sub,
            PermissionLevel.Mod,
            PermissionLevel.Own
        };

        private readonly CommandSetup _commandSetup;
        private readonly RelayCommand _applyCommand;

        private string _commandName;
        private string _description;
        private bool _isDirty;
        private PermissionLevel _permissionLevel;

        public string CommandName
        {
            get { return _commandName; }
            set { 
                _commandName = value.Trim(); 
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

        public CommandSetupDocumentViewModel(IMacroInstructionTemplateFactoryFacade macroInstructionTemplateFactoryFacade, IDocumentManager documentManager, CommandSetup commandSetup, SwitchConnector switchConnector, ISwitchControllerSelector switchControllerSelector, IDialogService dialogService) : base(documentManager)
        {
            _commandSetup = commandSetup;
            _commandName = commandSetup.Name;
            _description = commandSetup.Description;
            _permissionLevel = commandSetup.PermissionLevel;

            MacroTimeLineViewModel = new MacroTimeLineViewModel(macroInstructionTemplateFactoryFacade, commandSetup.Macro, switchControllerSelector, switchConnector, dialogService);

            Title = _commandName;

            _applyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);

            Validate();
        }

        private bool CanExecuteApplyCommand() => IsValid;

        private void ExecuteApplyCommand()
        {
            if (!IsActive)
                return;

            _commandSetup.Name = _commandName.Trim();
            _commandSetup.Description = _description.Trim();
            _commandSetup.PermissionLevel = _permissionLevel;

            MacroTimeLineViewModel.ApplyChanges();

            IsDirty = false;

        }

        public bool Validate()
        {
            IsValid = !string.IsNullOrWhiteSpace(CommandName);

            _applyCommand.NotifyCanExecuteChanged();

            return IsValid;
        }

    }
}
