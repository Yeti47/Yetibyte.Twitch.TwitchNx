using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public class ProjectSerializer
    {
        public ProjectData BuildProjectData(Project project)
        {
            ProjectData projectData = new ProjectData
            {
                Name = project.Name,
                CommandSettings = project.CommandSettings,
                SwitchBridgeAddress = project.SwitchBridgeClientConnectionSettings.Address,
                SwitchBridgePort = project.SwitchBridgeClientConnectionSettings.Port
            };

            return projectData;
        }

        public byte[] SerializeProjectData(ProjectData projectData)
        {
            string json = JsonSerializer.Serialize(projectData, new JsonSerializerOptions { WriteIndented = true });

            return Encoding.UTF8.GetBytes(json);
        }

        public ProjectData DeserializeProjectData(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);

            ProjectData? projectData = JsonSerializer.Deserialize<ProjectData>(json) ?? new ProjectData();

            return projectData;
        }

        public Project RestoreProject(ProjectData projectData)
        {
            SwitchBridge.SwitchBridgeClientConnectionSettings switchBridgeClientConnectionSettings = new SwitchBridge.SwitchBridgeClientConnectionSettings(
                projectData.SwitchBridgeAddress,
                projectData.SwitchBridgePort
            );

            Project project = new Project(projectData.Name, projectData.CommandSettings, switchBridgeClientConnectionSettings);

            return project;
        }
    }
}
