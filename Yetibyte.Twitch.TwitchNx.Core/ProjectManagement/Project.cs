using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public class Project : ICommandSettingsProvider, IProject
    {
        public const string UNTITLED_PROJECT_NAME = "Untitled";

        private readonly List<ICommandSourceSettings> _commandSourceSettings = new List<ICommandSourceSettings>();

        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }
            set
            {

                string newValue = value?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(newValue))
                    newValue = UNTITLED_PROJECT_NAME;

                if (_name != newValue)
                {
                    string oldName = _name;
                    _name = newValue;

                    OnNameChanged(oldName, Name);
                }
            }
        }

        public ICommandSourceFactory? CommandSourceFactory { get; set; }

        public IEnumerable<ICommandSourceSettings> CommandSourceSettings => _commandSourceSettings;

        public CommandSettings CommandSettings { get; } = new CommandSettings();

        public SwitchBridgeClientConnectionSettings SwitchBridgeClientConnectionSettings { get; } = SwitchBridgeClientConnectionSettings.CreateEmpty();


        public event EventHandler<NameChangedEventArgs>? NameChanged;

        public Project(string projectName)
        {
            _name = projectName;
        }

        public Project(string projectName, CommandSettings commandSettings, SwitchBridgeClientConnectionSettings switchBridgeClientConnectionSettings)
        {
            _name = projectName;
            CommandSettings = commandSettings;
            SwitchBridgeClientConnectionSettings = switchBridgeClientConnectionSettings;
        }

        protected virtual void OnNameChanged(string oldName, string newName)
        {
            var handler = NameChanged;
            handler?.Invoke(this, new NameChangedEventArgs(oldName, newName));
        }

        public void WriteCommandSourceSettings(ICommandSourceSettings commandSourceSettings)
        {
            if (_commandSourceSettings
                .FirstOrDefault(c => c.CommandSourceType == commandSourceSettings.CommandSourceType 
                                     && c.CommandSourceId == commandSourceSettings.CommandSourceId) 
                is ICommandSourceSettings existingSettings)
            {
                _commandSourceSettings.Remove(existingSettings);
            }

            _commandSourceSettings.Add(commandSourceSettings);
        }

        public ICommandSourceSettings? ReadCommmandSourceSettings(Type commandSourceType, string commandSourceId = "")
        {
            return _commandSourceSettings
                .FirstOrDefault(c => c.CommandSourceType == commandSourceType 
                                     && (string.IsNullOrWhiteSpace(commandSourceId) || c.CommandSourceId == commandSourceId)
            );
        }
    }
}
