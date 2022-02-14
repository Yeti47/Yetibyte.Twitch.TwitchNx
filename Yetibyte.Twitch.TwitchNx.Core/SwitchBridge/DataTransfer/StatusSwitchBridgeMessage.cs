namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record StatusSwitchBridgeMessage(string Id) : SwitchBridgeMessage(Id, MESSAGE_TYPE)
    {
        public const string MESSAGE_TYPE = "GET_STATUS";

        public StatusPayload Payload { get; init; } = new StatusPayload(string.Empty, Array.Empty<ControllerState>());
    }
}
