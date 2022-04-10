using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.SessionManagement
{
    public class SessionStoppedEventArgs : EventArgs
    {
        public DateTime EndTime { get; }


        public SessionStoppedEventArgs(DateTime endTime)
        {
            EndTime = endTime;
        }
    }

}
