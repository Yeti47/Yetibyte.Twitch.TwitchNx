namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchConnectorMacroEventargs : EventArgs
    {
        public string MacroMessageId { get; }

        public SwitchConnectorMacroEventargs(string macroMessageId)
        {
            MacroMessageId = macroMessageId;
        }
    }
}
