using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public class ProjectManager : IProjectManager
    {
        public const string PROJECT_FILE_EXTENSION = ".tnx";

        private Project? _currentProject;
        private string _projectFilePath = string.Empty;

        public ICommandSource? CommandSource { get; set; }

        public bool HasCommandSource => CommandSource is not null;

        public Project? CurrentProject
        {
            get { return _currentProject; }
            private set
            {

                if (_currentProject != value)
                {
                    OnProjectChanging();
                    _currentProject = value;
                    OnProjectChanged();
                }

            }
        }

        public bool IsProjectOpen => CurrentProject is not null;

        public string ProjectFilePath { 
            get => _projectFilePath;
            set => _projectFilePath = value?.Trim() ?? string.Empty; 
        }

        public bool IsFilePathSpecified => !string.IsNullOrWhiteSpace(ProjectFilePath);

        public bool IsFilePersisted
        {
            get
            {
                if (!IsFilePathSpecified)
                    return false;

                string filePath = ProjectFilePath;

                if (!filePath.EndsWith(PROJECT_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                    filePath += PROJECT_FILE_EXTENSION;

                return File.Exists(filePath);
            }
        }

        public event EventHandler? ProjectChanged;
        public event EventHandler? ProjectChanging;

        public ProjectManager()
        {

        }

        protected virtual void OnProjectChanged()
        {
            var handler = ProjectChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnProjectChanging()
        {
            var handler = ProjectChanging;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public Project OpenNewProject(string projectName)
        {
            Project project = new Project(projectName);
            project.SwitchBridgeClientConnectionSettings.Port = SwitchBridge.SwitchBridgeClientConnectionSettings.DEFAULT_PORT;

            CurrentProject = project;

            return project;
        }

        public bool SaveProject()
        {
            if (!IsProjectOpen || CurrentProject is null)
                return false;

            if (!IsFilePathSpecified)
            {
                throw new InvalidOperationException("No project file path specified.");
            }

            ProjectSerializer projectSerializer = new ProjectSerializer();

            ProjectData projectData = projectSerializer.BuildProjectData(CurrentProject);

            byte[] fileData = projectSerializer.SerializeProjectData(projectData);

            string filePath = ProjectFilePath;

            if (!filePath.EndsWith(PROJECT_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                filePath += PROJECT_FILE_EXTENSION;

            File.WriteAllBytes(filePath, fileData);

            return true;
        }

        public void CloseProject()
        {
            CurrentProject = null;
        }

        public bool OpenProject(string filePath)
        {
            if (IsProjectOpen)
                CloseProject();

            filePath = filePath.Trim();

            if (!filePath.EndsWith(PROJECT_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                filePath += PROJECT_FILE_EXTENSION;

            byte[] fileData = File.ReadAllBytes(filePath);

            ProjectSerializer projectSerializer = new ProjectSerializer();

            ProjectData projectData = projectSerializer.DeserializeProjectData(fileData);

            Project project = projectSerializer.RestoreProject(projectData);

            ProjectFilePath = filePath;
            CurrentProject = project;

            return true;
        }
    }
}
