namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record RemoveControllerSwitchBridgeMessage(string Id, ControllerPayload Payload) : SwitchBridgeMessage(Id, MESSAGE_TYPE)
    {
        public const string MESSAGE_TYPE = "REMOVE_CONTROLLER";
    }
}
