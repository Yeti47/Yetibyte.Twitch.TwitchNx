using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class CooldownGroupDocumentViewModel : DocumentViewModel
    {
        private readonly CooldownGroup _cooldownGroup;
        private readonly IProjectManager _projectManager;
        private readonly RelayCommand _applyCommand;
        
        private string _cooldownGroupName;
        private float _sharedTime;

        private float _levelTimeAnyone;
        private float _levelTimeSubscriber;
        private float _levelTimeModerator;
        private float _levelTimeChannelOwner;

        private string _userLevelMessage;
        private string _sharedMessage;

        private bool _isDirty;

        public ICommand ApplyCommand => _applyCommand;

        public override string DocumentName => _cooldownGroup.Name;

        public override Type DocumentType => _cooldownGroup.GetType();

        public bool IsValid { get; private set; }

        public string CooldownGroupName
        {
            get => _cooldownGroupName;
            set
            {
                _cooldownGroupName = value;
                OnPropertyChanged();

                IsDirty = true;

                Validate();
            }
        }

        public float SharedTime
        {
            get => _sharedTime;
            set
            {
                _sharedTime = value;
                OnPropertyChanged();

                IsDirty = true;

                Validate();
            }
        }

        public float LevelTimeAnyone
        {
            get => _levelTimeAnyone;
            set
            {
                _levelTimeAnyone = value;
                OnPropertyChanged();

                Validate();
            }
        }

        public float LevelTimeSubscriber
        {
            get => _levelTimeSubscriber;
            set
            {
                _levelTimeSubscriber = value;
                OnPropertyChanged();

                Validate();
            }
        }

        public float LevelTimeModerator
        {
            get => _levelTimeModerator;
            set
            {
                _levelTimeModerator = value;
                OnPropertyChanged();

                Validate();
            }
        }

        public float LevelTimeChannelOwner
        {
            get => _levelTimeChannelOwner;
            set
            {
                _levelTimeChannelOwner = value;
                OnPropertyChanged();

                Validate();
            }
        }

        public string UserLevelMessage
        {
            get => _userLevelMessage;
            set
            {
                _userLevelMessage = value;
                OnPropertyChanged();

                Validate();
            }
        }

        public string SharedMessage
        {
            get => _sharedMessage;
            set
            {
                _sharedMessage = value;
                OnPropertyChanged();

                Validate();
            }
        }
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;

                OnPropertyChanged();

                Title = _cooldownGroup.Name + (_isDirty ? "*" : string.Empty);
            }
        }

        public CooldownGroupDocumentViewModel(CooldownGroup cooldownGroup, IDocumentManager documentManager, IProjectManager projectManager) : base(documentManager)
        {
            _cooldownGroup = cooldownGroup;
            _projectManager = projectManager;
            Title = DocumentName;

            _cooldownGroupName = cooldownGroup.Name;
            _sharedTime = cooldownGroup.SharedTime;

            _levelTimeAnyone = cooldownGroup.GetTime(PermissionLevel.Any);
            _levelTimeSubscriber = cooldownGroup.GetTime(PermissionLevel.Sub);
            _levelTimeModerator = cooldownGroup.GetTime(PermissionLevel.Mod);
            _levelTimeChannelOwner = cooldownGroup.GetTime(PermissionLevel.Own);

            _userLevelMessage = cooldownGroup.UserCooldownMessage;
            _sharedMessage = cooldownGroup.SharedCooldownMessage;

            _applyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);

            Validate();
        }

        private bool CanExecuteApplyCommand() => IsValid;

        private void ExecuteApplyCommand()
        {
            if (!IsActive)
                return;

            CooldownGroupName = CooldownGroupName.Trim();

            _cooldownGroup.Name = CooldownGroupName;
            _cooldownGroup.SharedTime = SharedTime;

            _cooldownGroup.SetTime(PermissionLevel.Any, _levelTimeAnyone);
            _cooldownGroup.SetTime(PermissionLevel.Sub, _levelTimeSubscriber);
            _cooldownGroup.SetTime(PermissionLevel.Mod, _levelTimeModerator);
            _cooldownGroup.SetTime(PermissionLevel.Own, _levelTimeChannelOwner);

            UserLevelMessage = UserLevelMessage.Trim();
            SharedMessage = SharedMessage.Trim();

            _cooldownGroup.UserCooldownMessage = UserLevelMessage;
            _cooldownGroup.SharedCooldownMessage = SharedMessage;

            IsDirty = false;
        }

        public bool Validate()
        {
            IsValid = !string.IsNullOrWhiteSpace(CooldownGroupName);

            if (_projectManager.CurrentProject is not null)
                IsValid &= !_projectManager.CurrentProject.CommandSettings.CooldownGroups.Any(c => c != _cooldownGroup && c.Name.Equals(CooldownGroupName, StringComparison.OrdinalIgnoreCase));

            _applyCommand.NotifyCanExecuteChanged();

            return IsValid;
        }
    }
}
