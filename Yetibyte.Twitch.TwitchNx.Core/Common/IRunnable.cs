using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.Common
{
    public interface IRunnable
    {
        event EventHandler? Started;
        event EventHandler? Stopped;

        bool IsRunning { get; }

        bool Start();

        bool Stop();

    }
}
