namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class CooldownGroupAddedEventArgs : EventArgs
    {
        public CooldownGroup CooldownGroup { get; }

        public CooldownGroupAddedEventArgs(CooldownGroup cooldownGroup)
        {
            CooldownGroup = cooldownGroup;
        }

    }
}
