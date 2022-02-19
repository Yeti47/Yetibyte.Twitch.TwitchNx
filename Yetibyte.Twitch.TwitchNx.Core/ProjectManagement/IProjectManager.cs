using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public interface IProjectManager
    {
        Project? CurrentProject { get; }

        bool IsProjectOpen { get; }

        event EventHandler? ProjectChanged;
        event EventHandler? ProjectChanging;
    }
}
