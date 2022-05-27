using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    [Serializable]
    public class ProjectData
    {
        public string Name { get; set; } = string.Empty;

        public CommandSettings CommandSettings { get; set; } = new CommandSettings();

        public string SwitchBridgeAddress { get; set; } = string.Empty;
        public int SwitchBridgePort { get; set; }

        [Newtonsoft.Json.JsonProperty(ItemTypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects)]
        public List<ICommandSourceSettings> CommandSourceSettings { get; private set; } = new List<ICommandSourceSettings>();

        public string CommandSourceFactory { get; set; } = string.Empty;

    }
}
