using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public interface IProject
    {
        //ICommandSource? CommandSource { get; set; }
        ICommandSourceFactory? CommandSourceFactory { get; set; }

        CommandSettings CommandSettings { get; }
        string Name { get; set; }
        SwitchBridgeClientConnectionSettings SwitchBridgeClientConnectionSettings { get; }

        event EventHandler<NameChangedEventArgs>? NameChanged;
    }
}