namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record MacroCompleteSwitchBridgeMessage(string Id, MacroPayload Payload) : SwitchBridgeMessage(Id, MESSAGE_TYPE)
    {
        public const string MESSAGE_TYPE = "MACRO_COMPLETE";
    }
}
