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
        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }
            set { 
                if (_name != value)
                {
                    string oldName = _name;
                    _name = value; 

                    OnNameChanged(oldName, Name);
                }
            }
        }

        public bool IsUnnamed => string.IsNullOrWhiteSpace(_name);

        public CommandSettings CommandSettings { get; } = new CommandSettings();

        public SwitchBridgeClientConnectionSettings SwitchBridgeClientConnectionSettings { get; } = SwitchBridgeClientConnectionSettings.CreateEmpty();


        public event EventHandler<NameChangedEventArgs>? NameChanged;

        public Project()
        {
            
        }

        protected virtual void OnNameChanged(string oldName, string newName)
        {
            var handler = NameChanged;
            handler?.Invoke(this, new NameChangedEventArgs(oldName, newName));
        }

    }
}
