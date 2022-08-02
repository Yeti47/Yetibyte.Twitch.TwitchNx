namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeClientConnectionSettings
    {
        public const int DEFAULT_PORT = 4769;

        private string _address;
        private int _port;
        private bool _isMockingEnabled = false;
        private float _mockConnectionTimeSeconds = 1f;
        private float _mockDisconnectionTimeSeconds = 1f;

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

        public bool IsMockingEnabled
        {
            get => _isMockingEnabled;
            set
            {
                _isMockingEnabled = value;
                OnSettingsChanged();
            }
        }

        public float MockConnectionTimeSeconds
        {
            get => _mockConnectionTimeSeconds;
            set
            {
                _mockConnectionTimeSeconds = value;
                OnSettingsChanged();
            }
        }

        public float MockDisconnectionTimeSeconds
        {
            get => _mockDisconnectionTimeSeconds;
            set
            {
                _mockDisconnectionTimeSeconds = value;
                OnSettingsChanged();
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
