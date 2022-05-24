using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.Logging
{
    public class EventLogAppender : AppenderSkeleton
    {
        public event EventHandler<LoggingEventAppendedEventArgs>? LoggingEventAppended;

        protected override void Append(LoggingEvent loggingEvent)
        {
            OnLoggingEventAppended(loggingEvent);
        }

        protected virtual void OnLoggingEventAppended(LoggingEvent loggingEvent)
        {
            var handler = LoggingEventAppended;
            handler?.Invoke(this, new LoggingEventAppendedEventArgs(loggingEvent));
        }
    }
}
