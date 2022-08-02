namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class WebSocketSwitchBridgeClientFactory : ISwitchBridgeClientFactory
    {
        public ISwitchBridgeClient CreateClient(SwitchBridgeClientConnectionSettings connectionSettings)
        {
            return new SwitchBridgeClient(connectionSettings.Address, connectionSettings.Port);
        }
    }
}
