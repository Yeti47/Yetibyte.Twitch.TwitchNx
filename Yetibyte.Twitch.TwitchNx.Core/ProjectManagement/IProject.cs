using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public interface IProject
    {
        CommandSettings CommandSettings { get; }
        string Name { get; set; }
        SwitchBridgeClientConnectionSettings SwitchBridgeClientConnectionSettings { get; }

        event EventHandler<NameChangedEventArgs>? NameChanged;
    }
}