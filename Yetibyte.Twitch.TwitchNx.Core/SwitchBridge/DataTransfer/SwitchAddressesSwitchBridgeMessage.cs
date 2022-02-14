namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record SwitchAddressesSwitchBridgeMessage(string Id) : SwitchBridgeMessage(Id, MESSAGE_TYPE)
    {
        public const string MESSAGE_TYPE = "GET_SWITCH_ADDRESSES";

        public SwitchAddressPayload Payload { get; init; } = new SwitchAddressPayload(Array.Empty<string>());
    }
}
