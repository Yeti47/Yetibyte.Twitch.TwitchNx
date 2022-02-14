using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchControllerAddedEventArgs : EventArgs
    {
        public SwitchController Controller { get; }

        public SwitchControllerAddedEventArgs(SwitchController controller)
        {
            Controller = controller;
        }
    }
}
