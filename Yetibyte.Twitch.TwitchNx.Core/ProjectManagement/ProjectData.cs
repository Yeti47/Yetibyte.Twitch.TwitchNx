using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    [Serializable]
    public class ProjectData
    {
        public string Name { get; set; } = string.Empty;

        public CommandSettings CommandSettings { get; set; } = new CommandSettings();

        public string SwitchBridgeAddress { get; set; } = string.Empty;
        public int SwitchBridgePort { get; set; }


    }
}
