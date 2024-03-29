﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.Irc;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class IrcCommandSourceSettingsViewModel : ICommandSourceSettingsViewModel
    {
        class ToggleAuthTokenCommand : ICommand
        {
            private readonly IrcCommandSourceSettingsViewModel _ircCommandSourceSettings;

            public ToggleAuthTokenCommand(IrcCommandSourceSettingsViewModel ircCommandSourceSettings)
            {
                _ircCommandSourceSettings = ircCommandSourceSettings;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter) => true;

            public void Execute(object? parameter) => _ircCommandSourceSettings.IsAuthTokenVisible = !_ircCommandSourceSettings.IsAuthTokenVisible;
        }

        private string _userName = string.Empty;
        private string _channelName = string.Empty;
        private string _authToken = string.Empty;

        private bool _isAuthTokenVisible;
        private readonly IDirtiable _parentViewModel;

        public bool IsAuthTokenVisible
        {
            get { return _isAuthTokenVisible; }
            set { 
                _isAuthTokenVisible = value; 
                OnPropertyChanged();
            }
        }

        public string AuthToken
        {
            get { return _authToken; }
            set { 
                _authToken = value; 
                OnPropertyChanged(); 
                _parentViewModel.MarkDirty(); 
            }
        }

        public string ChannelName
        {
            get { return _channelName; }
            set { 
                _channelName = value; 
                OnPropertyChanged();
                _parentViewModel.MarkDirty();
            }
        }
        public string UserName
        {
            get { return _userName; }
            set { 
                _userName = value; 
                OnPropertyChanged();
                _parentViewModel.MarkDirty();
            }
        }

        public ICommand ToggleAuthTokenVisibilityCommand { get; }


        public event PropertyChangedEventHandler? PropertyChanged;


        public IrcCommandSourceSettingsViewModel(IDirtiable parentViewModel)
        {
            ToggleAuthTokenVisibilityCommand = new ToggleAuthTokenCommand(this);
            _parentViewModel = parentViewModel;
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(propertyName);

            var handler = PropertyChanged;

            handler?.Invoke(this, propertyChangedEventArgs);
        }

    }
}
