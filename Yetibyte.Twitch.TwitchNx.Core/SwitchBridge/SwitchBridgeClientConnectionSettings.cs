namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeClientConnectionSettings
    {
        public const int DEFAULT_PORT = 4769;

        private string _address;
        private int _port;

        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnSettingsChanged();
                }
            }
        }
        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnSettingsChanged();
                }
            }
        }

        public event EventHandler? SettingsChanged;

        public SwitchBridgeClientConnectionSettings(string address, int port = DEFAULT_PORT)
        {
            _address = address;
            _port = port;
        }

        public static SwitchBridgeClientConnectionSettings CreateEmpty() => new SwitchBridgeClientConnectionSettings("", 0);

        protected virtual void OnSettingsChanged()
        {
            var handler = SettingsChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

    }
}
