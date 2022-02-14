using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public interface ISwitchBridgeClient : IDisposable
    {
        event EventHandler<SwitchBridgeMessageReceivedEventArgs>? MessageReceived;
        event EventHandler? Connected;
        event EventHandler? Disconnected;

        SwitchBridgeClientConnectionSettings ConnectionSettings { get; set; }

        bool IsConnected { get; }

        bool Connect();
        bool Disconnect();

        void CreateController(string controllerType);
        void RemoveController(int controllerId);

        void ExecuteMacro(string macro, int controllerId);

        void GetSwitchStatus();

        void GetSwitchAddresses();
        
    }
}
