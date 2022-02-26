using System.Text;
using Newtonsoft.Json;

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
            //string json = JsonSerializer.Serialize(projectData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, IgnoreReadOnlyFields = false });

            string json = JsonConvert.SerializeObject(projectData, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return Encoding.UTF8.GetBytes(json);
        }

        public ProjectData DeserializeProjectData(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);

            //ProjectData? projectData = JsonSerializer.Deserialize<ProjectData>(json, new JsonSerializerOptions { IncludeFields = true, IgnoreReadOnlyFields = false }) ?? new ProjectData();

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

            return project;
        }
    }
}
