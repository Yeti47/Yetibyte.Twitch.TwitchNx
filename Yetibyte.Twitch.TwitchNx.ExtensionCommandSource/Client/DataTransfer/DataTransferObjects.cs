using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.ExtensionCommandSource.Client.DataTransfer
{
    [Serializable]
    public record Command(string Name, string Description, string CooldownGroup);

    [Serializable]
    public record CooldownGroup(string Name, double SharedTime);

    [Serializable]
    public record CommandRequest(string TwitchNxClientId, string MessageType, Command[] Commands, CooldownGroup[] CooldownGroups);

    [Serializable]
    public record SetupCommandsResponse(int CommandCount, int CooldownCount);

    [Serializable]
    public record UserCommand(string Name, string User, DateTime Time);

    [Serializable]
    public record ReceiveUserCommandsResponse(UserCommand[] Commands);

}
