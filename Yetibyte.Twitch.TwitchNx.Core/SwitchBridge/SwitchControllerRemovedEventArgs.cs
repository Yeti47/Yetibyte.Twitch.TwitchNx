namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchControllerRemovedEventArgs : EventArgs
    {
        public int ControllerId { get; }

        public SwitchControllerRemovedEventArgs(int controllerId)
        {
            ControllerId = controllerId;
        }
    }
}
