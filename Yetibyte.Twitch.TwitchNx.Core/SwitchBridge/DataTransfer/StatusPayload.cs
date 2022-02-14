namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record StatusPayload(string Status, ControllerState[] ControllerStates);
}
