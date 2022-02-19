using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.Utilities;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class CommandSettings
    {
        private const string DEFAULT_COMMAND_PREFIX = "!";

        private readonly List<CommandSetup> _commands = new List<CommandSetup>();
        private readonly List<CooldownGroup> _cooldownGroups = new List<CooldownGroup>();

        public string CommandPrefix { get; set; } = DEFAULT_COMMAND_PREFIX;

        public IReadOnlyList<CommandSetup> CommandSetups => new ReadOnlyCollection<CommandSetup>(_commands);
        public IReadOnlyList<CooldownGroup> CooldownGroups => new ReadOnlyCollection<CooldownGroup>(_cooldownGroups);

        public event EventHandler<CommandSetupAddedEventArgs>? CommandSetupAdded;
        public event EventHandler<CommandSetupRemovedEventArgs>? CommandSetupRemoved;

        public event EventHandler<CooldownGroupAddedEventArgs>? CooldownGroupAdded;
        public event EventHandler<CooldownGroupRemovedEventArgs>? CooldownGroupRemoved;

        public CommandSettings()
        {
        }

        public void AddCommandSetup(CommandSetup commandSetup)
        {
            if (_commands.Contains(commandSetup))
                return;

            _commands.Add(commandSetup);

            OnCommandSetupAdded(commandSetup);
        }

        public bool RemoveCommandSetup(CommandSetup commandSetup)
        {
            bool success = _commands.Remove(commandSetup);

            if (success)
                OnCommandSetupRemoved(commandSetup);

            return success;
        }

        public void ClearCommandSetups()
        {
            _commands.MutableForeach(c => RemoveCommandSetup(c));
        }

        public bool HasCommand(CommandSetup commandSetup) => _commands.Contains(commandSetup);

        public bool HasCommand(string command) => _commands.Any(c => c.Command == command);

        public void AddCooldownGroup(CooldownGroup cooldownGroup)
        {
            if (_cooldownGroups.Contains(cooldownGroup))
                return;

            _cooldownGroups.Add(cooldownGroup);

            OnCooldownGroupAdded(cooldownGroup);
        }

        public bool RemoveCooldownGroup(CooldownGroup cooldownGroup)
        {
            bool success = _cooldownGroups.Remove(cooldownGroup);

            if (success)
                OnCooldownGroupRemoved(cooldownGroup);

            return success;
        }

        public void ClearCooldownGroups()
        {
            _cooldownGroups.MutableForeach(cg => RemoveCooldownGroup(cg));
        }

        public bool HasCooldownGroup(CooldownGroup cooldownGroup) => _cooldownGroups.Contains(cooldownGroup);

        public bool HasCooldownGroup(string name) => _cooldownGroups.Any(c => c.Name == name);

        protected virtual void OnCommandSetupAdded(CommandSetup commandSetup)
        {
            var handler = CommandSetupAdded;
            handler?.Invoke(this, new CommandSetupAddedEventArgs(commandSetup));
        }
        protected virtual void OnCommandSetupRemoved(CommandSetup commandSetup)
        {
            var handler = CommandSetupRemoved;
            handler?.Invoke(this, new CommandSetupRemovedEventArgs(commandSetup));
        }

        protected virtual void OnCooldownGroupAdded(CooldownGroup cooldownGroup)
        {
            var handler = CooldownGroupAdded;
            handler?.Invoke(this, new CooldownGroupAddedEventArgs(cooldownGroup));
        }

        protected virtual void OnCooldownGroupRemoved(CooldownGroup cooldownGroup)
        {
            var handler = CooldownGroupRemoved;
            handler?.Invoke(this, new CooldownGroupRemovedEventArgs(cooldownGroup));
        }
    }
}
