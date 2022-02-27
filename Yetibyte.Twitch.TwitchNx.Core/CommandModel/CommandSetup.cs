using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.Common;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    [Serializable]
    public class CommandSetup
    {
        private string _command = string.Empty;
        private string _name = string.Empty;

        public string Command
        {
            get => _command;
            set
            {
                if (_command != value)
                {
                    _command = value;
                }
            }
        }

        public string Name
        {
            get => string.IsNullOrWhiteSpace(_name) ? _command : _name;
            set
            {
                string oldName = _name;
                _name = value;
                OnNameChanged(oldName);
            }
        }

        public string Description { get; set; } = string.Empty;

        public Macro Macro { get; set; }

        public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.Any;

        public CooldownGroup? CooldownGroup { get; set; }

        public int ControllerIndex { get; set; } = 0;

        public event EventHandler<NameChangedEventArgs>? NameChanged;

        public CommandSetup(string command, Macro macro)
        {
            Command = command;
            Macro = macro;
        }

        public CommandSetup(string command) : this(command, new Macro())
        {

        }

        private CommandSetup() : this(string.Empty)
        {
        }

        protected virtual void OnNameChanged(string oldName)
        {
            var handler = NameChanged;
            handler?.Invoke(this, new NameChangedEventArgs(oldName, Name));
        }

    }
}
