namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class CooldownGroupRemovedEventArgs : EventArgs
    {
        public CooldownGroup CooldownGroup { get; }

        public CooldownGroupRemovedEventArgs(CooldownGroup cooldownGroup)
        {
            CooldownGroup = cooldownGroup;
        }

    }
}
