using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class AddingSwitchControllerEventArgs : EventArgs
    {

        public ControllerType ControllerType { get; }

        public AddingSwitchControllerEventArgs(ControllerType controllerType)
        {
            ControllerType = controllerType;
        }

    }
}
