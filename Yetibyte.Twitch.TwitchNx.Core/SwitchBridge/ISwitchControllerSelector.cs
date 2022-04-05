using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public interface ISwitchControllerSelector
    {
        event EventHandler? SelectedControllerChanged;

        SwitchController? SelectedController { get; }

        bool HasSelectedController { get; }
    }
}
