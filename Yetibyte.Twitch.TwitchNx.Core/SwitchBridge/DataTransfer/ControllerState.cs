namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record ControllerState(int Id, string State, string Type, string Errors, string[] FinishedMacros);
}
