using log4net;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public class ProjectManager : IProjectManager
    {
        public const string PROJECT_FILE_EXTENSION = ".tnx";
        private readonly ICommandSourceProvider _commandSourceProvider;
        private readonly ILog _logger;
        private Project? _currentProject;
        private string _projectFilePath = string.Empty;


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

        public ProjectManager(ICommandSourceProvider commandSourceProvider, ILog logger) 
        {
            _commandSourceProvider = commandSourceProvider;
            _logger = logger;
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

            _logger.Info($"Opened new project '{CurrentProject.Name}'.");

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

            ProjectSerializer projectSerializer = new ProjectSerializer(_commandSourceProvider);

            ProjectData projectData = projectSerializer.BuildProjectData(CurrentProject);

            byte[] fileData = projectSerializer.SerializeProjectData(projectData);

            string filePath = ProjectFilePath;

            if (!filePath.EndsWith(PROJECT_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                filePath += PROJECT_FILE_EXTENSION;

            File.WriteAllBytes(filePath, fileData);

            _logger.Info($"Saved project to file {filePath}.");

            return true;
        }

        public void CloseProject()
        {
            if (CurrentProject is null)
                return;

            string projectName = CurrentProject.Name;

            CurrentProject = null;

            _logger.Info($"Project '{projectName}' closed.");
        }

        public bool OpenProject(string filePath)
        {
            if (IsProjectOpen)
                CloseProject();

            filePath = filePath.Trim();

            if (!filePath.EndsWith(PROJECT_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                filePath += PROJECT_FILE_EXTENSION;

            byte[] fileData = File.ReadAllBytes(filePath);

            ProjectSerializer projectSerializer = new ProjectSerializer(_commandSourceProvider);

            ProjectData projectData = projectSerializer.DeserializeProjectData(fileData);

            Project project = projectSerializer.RestoreProject(projectData);

            ProjectFilePath = filePath;
            CurrentProject = project;

            _logger.Info($"Project '{CurrentProject.Name}' opened.");

            return true;
        }
    }
}
