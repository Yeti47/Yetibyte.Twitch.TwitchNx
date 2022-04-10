using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.ProjectManagement;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;

namespace Yetibyte.Twitch.TwitchNx.Core.SessionManagement
{
    public class SessionManager : ISessionManager
    {
        private readonly IProjectManager _projectManager;
        private readonly SwitchConnector _switchConnector;

        private CommandReceiver? _commandReceiver;

        public DateTime? SessionStartTime { get; private set; }

        public DateTime? SessionEndTime { get; private set; }

        public bool IsSessionRunning => _commandReceiver?.IsRunning ?? false;

        public event EventHandler<SessionStartedEventArgs>? SessionStarted;
        public event EventHandler<SessionStoppedEventArgs>? SessionStopped;

        public SessionManager(IProjectManager projectManager, SwitchConnector switchConnector)
        {
            _projectManager = projectManager;
            _switchConnector = switchConnector;
        }

        public bool StartSession()
        {
            if (IsSessionRunning)
            {
                throw new InvalidOperationException("Session already running.");
            }

            if(_projectManager.CurrentProject is null)
            {
                throw new InvalidOperationException("No project loaded.");
            }

            if(_projectManager.CurrentProject.CommandSource is null)
            {
                throw new InvalidOperationException("No command source provided.");
            }

            if(!_switchConnector.IsConnected)
            {
                throw new InvalidOperationException("Connection to Nintendo Switch console has not been established.");
            }

            //if (!_projectManager.CurrentProject.CommandSource.IsRunning)
            //{
            //    bool commandSourceStarted = _projectManager.CurrentProject.CommandSource.Start();

            //    if (!commandSourceStarted)
            //        return false;
            //}

            if (_commandReceiver is not null)
            {
                _commandReceiver.Started -= _commandReceiver_Started;
                _commandReceiver.Stopped -= _commandReceiver_Stopped;

                if(_commandReceiver.IsRunning)
                    _commandReceiver.Stop();
            }

            _commandReceiver = new CommandReceiver(
                _projectManager.CurrentProject.CommandSource,
                _switchConnector,
                _projectManager.CurrentProject.SwitchBridgeClientConnectionSettings,
                _projectManager.CurrentProject.CommandSettings,
                null
            );

            _commandReceiver.Started += _commandReceiver_Started;
            _commandReceiver.Stopped += _commandReceiver_Stopped;

            _commandReceiver.Start();

            return true;
        }

        private void _commandReceiver_Stopped(object? sender, EventArgs e)
        {
            OnSessionStopped();
        }

        private void _commandReceiver_Started(object? sender, EventArgs e)
        {
            OnSessionStarted();
        }

        public bool StopSession()
        {
            if (!IsSessionRunning || _commandReceiver is null)
                return false;

            _commandReceiver.Stop();

            return true;
        }

        protected virtual void OnSessionStarted()
        {
            SessionStartedEventArgs sessionStartedEventArgs = new SessionStartedEventArgs(DateTime.Now);

            var handler = SessionStarted;
            handler?.Invoke(this, sessionStartedEventArgs);
        }

        protected virtual void OnSessionStopped()
        {
            SessionStoppedEventArgs sessionStoppedEventArgs = new SessionStoppedEventArgs(DateTime.Now);

            var handler = SessionStopped;
            handler?.Invoke(this, sessionStoppedEventArgs);
        }

    }
}
