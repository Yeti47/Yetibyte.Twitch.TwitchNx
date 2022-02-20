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
