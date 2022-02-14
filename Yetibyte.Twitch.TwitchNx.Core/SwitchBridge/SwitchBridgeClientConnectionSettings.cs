namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeClientConnectionSettings
    {
        public string Address { get; set; }
        public int Port { get; set; }

        public SwitchBridgeClientConnectionSettings(string address, int port = 4769)
        {
            Address = address;
            Port = port;
        }

        public static SwitchBridgeClientConnectionSettings CreateEmpty() => new SwitchBridgeClientConnectionSettings("", 0);

    }
}
