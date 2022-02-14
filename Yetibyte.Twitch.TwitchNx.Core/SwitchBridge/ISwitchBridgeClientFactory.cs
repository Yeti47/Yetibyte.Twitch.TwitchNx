namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public interface ISwitchBridgeClientFactory
    {
        ISwitchBridgeClient CreateClient(SwitchBridgeClientConnectionSettings connectionSettings);
    }
}
