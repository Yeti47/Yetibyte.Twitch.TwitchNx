using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeMessageReceivedEventArgs : EventArgs
    {
        public SwitchBridgeMessage Message { get; private set; }

        public SwitchBridgeMessageReceivedEventArgs(SwitchBridgeMessage message)
        {
            Message = message;
        }

    }
}
