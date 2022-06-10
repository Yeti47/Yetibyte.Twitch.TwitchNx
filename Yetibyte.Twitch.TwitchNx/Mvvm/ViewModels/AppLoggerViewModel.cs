using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.Logging;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;
using Yetibyte.Twitch.TwitchNx.Styling;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    [DefaultPane("MainBottom")]
    public class AppLoggerViewModel : ToolViewModel
    {
        public class LogEventViewModel : ObservableObject
        {
            private bool _isVisible = true;

            public int Level { get; }
            public string LevelName { get; }
            public DateTime TimeStamp { get; }
            public string Message { get; }

            public bool IsVisible
            {
                get => _isVisible;
                set
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }

            public LogEventViewModel(int level, string levelName, DateTime timeStamp, string message)
            {
                Level = level;
                LevelName = levelName;
                TimeStamp = timeStamp;
                Message = message;
            }

        }

        public class LogLevelViewModel : ObservableObject
        {
            private bool _isEnabled = true;

            public int Level { get; }
            public string Name { get; }

            public bool IsEnabled
            {
                get => _isEnabled;
                set
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                    IsEnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public event EventHandler? IsEnabledChanged;

            public LogLevelViewModel(int level, string name)
            {
                Level = level;
                Name = name;
            }

        }

        private readonly ObservableCollection<LogEventViewModel> _logEvents = new ObservableCollection<LogEventViewModel>();

        private readonly List<LogLevelViewModel> _logLevels = new List<LogLevelViewModel>
        {
            new LogLevelViewModel(log4net.Core.Level.Info.Value, log4net.Core.Level.Info.Name),
            new LogLevelViewModel(log4net.Core.Level.Warn.Value, log4net.Core.Level.Warn.Name),
            new LogLevelViewModel(log4net.Core.Level.Error.Value, log4net.Core.Level.Error.Name)
        };

        private readonly EventLogAppender _eventLogAppender;

        private readonly RelayCommand _clearCommand;

        public override bool DisableDuringSession => false;

        public ICommand ClearCommand => _clearCommand;

        public IEnumerable<LogEventViewModel> LogEvents => _logEvents;

        public IEnumerable<LogLevelViewModel> LogLevels => _logLevels;

        public AppLoggerViewModel(EventLogAppender eventLogAppender) : base("Logger")
        {
            _eventLogAppender = eventLogAppender;

            _clearCommand = new RelayCommand(() =>
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    _logEvents.Clear();
                });
            });

            _eventLogAppender.LoggingEventAppended += _eventLogAppender_LoggingEventAppended;

            foreach(var logLevel in _logLevels)
            {
                logLevel.IsEnabledChanged += LogLevel_IsEnabledChanged;
            }
        }

        private void LogLevel_IsEnabledChanged(object? sender, EventArgs e)
        {
            if (sender is not LogLevelViewModel logLevelViewModel)
                return;

            foreach(var logEvent in _logEvents.Where(le => le.Level == logLevelViewModel.Level))
            {
                logEvent.IsVisible = logLevelViewModel.IsEnabled;
            }
        }

        private void _eventLogAppender_LoggingEventAppended(object? sender, LoggingEventAppendedEventArgs e)
        {
            LogEventViewModel logEventViewModel = new LogEventViewModel(e.LoggingEvent.Level.Value, e.LoggingEvent.Level.Name, e.LoggingEvent.TimeStamp, e.LoggingEvent.RenderedMessage)
            {
                IsVisible = _logLevels.Any(l => l.Level == e.LoggingEvent.Level.Value && l.IsEnabled)
            };

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                _logEvents.Add(logEventViewModel);
            });
            
        }
    }
}
