using System.Text;
using Newtonsoft.Json;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public class ProjectSerializer
    {
        private readonly ICommandSourceProvider _commandSourceProvider;

        public ProjectSerializer(ICommandSourceProvider commandSourceProvider)
        {
            _commandSourceProvider = commandSourceProvider;
        }

        public ProjectData BuildProjectData(Project project)
        {
            ProjectData projectData = new ProjectData
            {
                Name = project.Name,
                CommandSettings = project.CommandSettings,
                SwitchBridgeAddress = project.SwitchBridgeClientConnectionSettings.Address,
                SwitchBridgePort = project.SwitchBridgeClientConnectionSettings.Port,
                CommandSourceFactory = project.CommandSourceFactory?.Id ?? string.Empty
            };

            projectData.CommandSourceSettings.AddRange(project.CommandSourceSettings);
                
            return projectData;
        }

        public byte[] SerializeProjectData(ProjectData projectData)
        {
            string json = JsonConvert.SerializeObject(projectData, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return Encoding.UTF8.GetBytes(json);
        }

        public ProjectData DeserializeProjectData(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);

            ProjectData projectData = JsonConvert.DeserializeObject<ProjectData>(json, new JsonSerializerSettings { }) ?? new ProjectData();

            return projectData;
        }

        public Project RestoreProject(ProjectData projectData)
        {
            SwitchBridge.SwitchBridgeClientConnectionSettings switchBridgeClientConnectionSettings = new SwitchBridge.SwitchBridgeClientConnectionSettings(
                projectData.SwitchBridgeAddress,
                projectData.SwitchBridgePort
            );

            Project project = new Project(projectData.Name, projectData.CommandSettings, switchBridgeClientConnectionSettings);

            foreach (var cmdSourceSettings in projectData.CommandSourceSettings)
                project.WriteCommandSourceSettings(cmdSourceSettings);

            project.CommandSourceFactory = _commandSourceProvider.GetCommandSourceFactories().FirstOrDefault(csf => csf.Id == projectData.CommandSourceFactory);

            return project;
        }
    }
}
