namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record MacroPayload(string Macro, int ControllerId);

    [Serializable]
    public record MacroCompletePayload(string MacroId, int ControllerId, string OriginalMessageId);
}
