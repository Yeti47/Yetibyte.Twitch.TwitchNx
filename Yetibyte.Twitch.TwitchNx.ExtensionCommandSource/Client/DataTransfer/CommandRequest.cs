using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Client.DataTransfer
{
    [Serializable]
    public class CommandRequest
    {
        [JsonPropertyName("TwitchStreamer")]
        public string TwitchStreamer { get; set; } = string.Empty;

        [JsonPropertyName("MsgType")]
        public string MessageType { get; set; } = string.Empty;
    }
}
