using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public interface IProjectManager
    {
        ICommandSource? CommandSource { get; set; }

        bool HasCommandSource { get; }

        Project? CurrentProject { get; }

        bool IsProjectOpen { get; }
        string ProjectFilePath { get; set; }
        bool IsFilePathSpecified { get; }
        bool IsFilePersisted { get; }

        event EventHandler? ProjectChanged;
        event EventHandler? ProjectChanging;

        void CloseProject();
        Project OpenNewProject(string projectName);
        bool OpenProject(string filePath);
        bool SaveProject();
    }
}
