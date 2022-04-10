using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.SessionManagement
{

    public interface ISessionManager
    {
        event EventHandler<SessionStartedEventArgs>? SessionStarted;
        event EventHandler<SessionStoppedEventArgs>? SessionStopped;

        DateTime? SessionStartTime { get; }
        DateTime? SessionEndTime { get; }

        bool IsSessionRunning { get; }

        bool StartSession();

        bool StopSession();
    }

}
