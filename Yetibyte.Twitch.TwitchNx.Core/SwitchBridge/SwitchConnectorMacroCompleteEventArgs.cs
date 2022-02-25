namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchConnectorMacroCompleteEventArgs : EventArgs
    {
        public string OriginalMacroMessageId { get; }

        public SwitchConnectorMacroCompleteEventArgs(string originalMacroMessageId)
        {
            OriginalMacroMessageId = originalMacroMessageId;
        }

    }
}
