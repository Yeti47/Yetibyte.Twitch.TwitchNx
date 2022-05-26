using log4net.Core;

namespace Yetibyte.Twitch.TwitchNx.Core.Logging
{
    public class LoggingEventAppendedEventArgs : EventArgs
    {
        public LoggingEvent LoggingEvent { get; }

        public LoggingEventAppendedEventArgs(LoggingEvent loggingEvent)
        {
            LoggingEvent = loggingEvent;
        }
    }
}
