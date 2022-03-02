using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Client.DataTransfer
{
    [Serializable]
    public class Command
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("desc")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("cdt")]
        public double CooldownTime { get; set; } = 0;
    }
}
