namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record CreateControllerSwitchBridgeMessage(string Id, ControllerPayload Payload) : SwitchBridgeMessage(Id, MESSAGE_TYPE)
    {
        public const string MESSAGE_TYPE = "CREATE_CONTROLLER";
    }
}
