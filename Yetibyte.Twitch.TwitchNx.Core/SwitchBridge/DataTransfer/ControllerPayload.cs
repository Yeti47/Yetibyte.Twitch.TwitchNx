namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record ControllerPayload(string ControllerType = "", int ControllerId = -1);
}
