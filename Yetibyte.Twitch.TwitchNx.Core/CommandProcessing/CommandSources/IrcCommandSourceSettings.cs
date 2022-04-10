using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.Irc;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class IrcCommandSourceSettings : INotifyPropertyChanged
    {
        class ToggleAuthTokenCommand : ICommand
        {
            private readonly IrcCommandSourceSettings _ircCommandSourceSettings;

            public ToggleAuthTokenCommand(IrcCommandSourceSettings ircCommandSourceSettings)
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

        public bool IsAuthTokenVisible
        {
            get { return _isAuthTokenVisible; }
            set { _isAuthTokenVisible = value; OnPropertyChanged(); }
        }

        public string AuthToken
        {
            get { return _authToken; }
            set { _authToken = value; OnPropertyChanged(); }
        }

        public string ChannelName
        {
            get { return _channelName; }
            set { _channelName = value; OnPropertyChanged(); }
        }
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }
        public IIrcClient IrcClient { get; }

        public ICommand ToggleAuthTokenVisibilityCommand { get; }


        public event PropertyChangedEventHandler? PropertyChanged;


        public IrcCommandSourceSettings(IIrcClient ircClient)
        {
            IrcClient = ircClient;
            ToggleAuthTokenVisibilityCommand = new ToggleAuthTokenCommand(this);
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(propertyName);

            var handler = PropertyChanged;

            handler?.Invoke(this, propertyChangedEventArgs);
        }

    }
}
