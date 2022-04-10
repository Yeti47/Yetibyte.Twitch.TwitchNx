using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.SessionManagement
{
    public class SessionStartedEventArgs : EventArgs
    {
        public DateTime StartTime { get; }

        public SessionStartedEventArgs(DateTime startTime)
        {
            StartTime = startTime;
        }
    }

}
