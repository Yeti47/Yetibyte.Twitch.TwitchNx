using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;
using Yetibyte.Twitch.TwitchNx.Core.Common;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;

namespace Yetibyte.Twitch.TwitchNx.Core.ProjectManagement
{
    public class Project
    {
        public const string UNTITLED_PROJECT_NAME = "Untitled";

        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }
            set { 

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

    }
}
