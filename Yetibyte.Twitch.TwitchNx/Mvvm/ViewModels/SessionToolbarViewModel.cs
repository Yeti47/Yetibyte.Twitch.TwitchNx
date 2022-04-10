using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.SessionManagement;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class SessionToolbarViewModel : ObservableObject
    {
        private readonly RelayCommand _startSessionCommand;
        private readonly RelayCommand _stopSessionCommand;

        public ISessionManager SessionManager { get; }

        public bool IsSessionRunning => SessionManager.IsSessionRunning;

        public ICommand StartSessionCommand => _startSessionCommand;
        public ICommand StopSessionCommand => _stopSessionCommand;

        public SessionToolbarViewModel(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;

            _startSessionCommand = new RelayCommand(ExecuteStartSessionCommand, CanExecuteStartSessionCommand);
            _stopSessionCommand = new RelayCommand(ExecuteStopSessionCommand, CanExecuteStopSessionCommand);

            sessionManager.SessionStarted += SessionManager_SessionStarted;
            sessionManager.SessionStopped += SessionManager_SessionStopped;
        }

        private bool CanExecuteStopSessionCommand()
        {
            return SessionManager.IsSessionRunning;
        }

        private void ExecuteStopSessionCommand()
        {
            SessionManager.StopSession();
        }

        private bool CanExecuteStartSessionCommand()
        {
            return !SessionManager.IsSessionRunning;
        }

        private void ExecuteStartSessionCommand()
        {
            try
            {
                SessionManager.StartSession();
            }
            catch(Exception ex)
            {
                // TODO: proper error handling!
            }
        }

        private void SessionManager_SessionStopped(object? sender, SessionStoppedEventArgs e)
        {
            NotifySessionStatusChanged();
        }

        private void NotifySessionStatusChanged()
        {
            OnPropertyChanged(nameof(IsSessionRunning));
            _startSessionCommand.NotifyCanExecuteChanged();
            _stopSessionCommand.NotifyCanExecuteChanged();
        }

        private void SessionManager_SessionStarted(object? sender, SessionStartedEventArgs e)
        {
            NotifySessionStatusChanged();
        }
    }
}
