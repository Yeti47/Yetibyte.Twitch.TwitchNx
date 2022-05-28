using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Core.Common;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    [Serializable]
    public class CommandSetup
    {
        private string _command = string.Empty;
        private string _name = string.Empty;
        private CooldownGroup? _cooldownGroup;

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

        public CooldownGroup? CooldownGroup
        {
            get => _cooldownGroup;
            set
            {
                _cooldownGroup = value;
                OnCooldownGroupChanged();
            }
        }

        public int ControllerIndex { get; set; } = 0;

        public event EventHandler<NameChangedEventArgs>? NameChanged;
        public event EventHandler? CooldownGroupChanged;

        public CommandSetup(string name, Macro macro)
        {
            Name = name;
            Macro = macro;
        }

        public CommandSetup(string name) : this(name, new Macro())
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

        protected virtual void OnCooldownGroupChanged()
        {
            var handler = CooldownGroupChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }


    }
}
