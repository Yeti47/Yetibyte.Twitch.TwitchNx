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

        public Macro Macro { get; set; }

        public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.Any;

        public string CooldownGroupName { get; set; } = string.Empty;

        public CommandSetup(string command, Macro macro)
        {
            Command = command;
            Macro = macro;
        }

    }
}
