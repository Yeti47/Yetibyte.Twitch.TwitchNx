namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public interface ICommandUser
    {
        string Name { get; }
        string DisplayName { get; }

        System.Drawing.Color Color { get; } 

        bool IsMod { get; }
        bool IsSubscriber { get; }
        bool IsOwner { get; }
    }
}
