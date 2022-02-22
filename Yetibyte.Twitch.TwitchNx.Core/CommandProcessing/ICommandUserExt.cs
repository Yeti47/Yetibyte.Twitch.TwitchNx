using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public static class ICommandUserExt
    {
        public static PermissionLevel GetPermissionLevel(this ICommandUser commandUser)
        {
            PermissionLevel permissionLevel = PermissionLevel.Any;

            if (commandUser.IsOwner)
                permissionLevel = PermissionLevel.Own;
            else if (commandUser.IsMod)
                permissionLevel = PermissionLevel.Mod;
            else if (commandUser.IsSubscriber)
                permissionLevel = PermissionLevel.Sub;

            return permissionLevel;
        }
    }
}
