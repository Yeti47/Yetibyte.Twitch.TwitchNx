namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public class ProjectManager : IProjectManager
    {
        private Project? _currentProject;

        public Project? CurrentProject
        {
            get { return _currentProject; }
            set { 

                if (_currentProject != value)
                {
                    OnProjectChanging();
                    _currentProject = value;
                    OnProjectChanged();
                }
                
            }
        }

        public bool IsProjectOpen => CurrentProject is not null;

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
    }
}
