using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer
{
    [Serializable]
    public record SwitchBridgeMessage(string Id, string MessageType)
    {
        public bool IsError { get; init; } = false;
        public string ErrorCode { get; init; } = string.Empty;
        public string ErrorMessage { get; init; } = string.Empty;
    };
}
