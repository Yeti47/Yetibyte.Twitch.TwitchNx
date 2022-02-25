using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public class CommandProcessor
    {
        private const double EPSILON = 0.0001;

        private readonly Dictionary<string, DateTime> _lastUses = new Dictionary<string, DateTime>();

        private readonly CommandSetup _commandSetup;
        private readonly ICommandQueue _commandQueue;

        private DateTime _sharedLastUse;

        public CommandProcessor(CommandSetup commandSetup, ICommandQueue commandQueue)
        {
            _commandSetup = commandSetup;
            _commandQueue = commandQueue;
        }

        public bool AppliesTo(Command command) => command.Name.Equals(_commandSetup.Command, StringComparison.OrdinalIgnoreCase);

        public DateTime GetLastUse(string userName)
        {
            return _lastUses.TryGetValue(userName, out DateTime useTime) ? useTime : default;
        }

        private void UpdateLastUse(string userName)
        {
            _lastUses[userName] = DateTime.Now;
        }

        public CommandProcessingResult Process(Command command)
        {
            if (!AppliesTo(command))
            {
                return new CommandProcessingResult(
                    Command: command,
                    Success: false,
                    IsMatch: false,
                    WasEnqueued: false
                );
            }

            double sharedSecondsRemaining = 0;

            if (_sharedLastUse != default)
            {
                var sharedTimeDelta = (DateTime.Now - _sharedLastUse);
                sharedSecondsRemaining = Math.Max(
                    (_commandSetup.CooldownGroup?.SharedTime).GetValueOrDefault() - sharedTimeDelta.TotalSeconds, 
                    0
                );

            }

            double userCooldownSeconds = _commandSetup.CooldownGroup?.GetTime(command.User.GetPermissionLevel()) ?? 0;

            DateTime userLastUse = GetLastUse(command.User.Name);

            double userSecondsRemaining = 0;

            if (userLastUse != default)
            {
                var timeDelta = (DateTime.Now - userLastUse);
                userSecondsRemaining = Math.Max(userCooldownSeconds - timeDelta.TotalSeconds, 0);
            }

            if (sharedSecondsRemaining > EPSILON || userSecondsRemaining > EPSILON)
            {
                return new CommandProcessingResult(
                    Command: command,
                    Success: false,
                    IsMatch: true,
                    WasEnqueued: false,
                    TimeRemaining: TimeSpan.FromSeconds(userSecondsRemaining),
                    SharedTimeRemaining: TimeSpan.FromSeconds(sharedSecondsRemaining)
                );
            }

            _sharedLastUse = DateTime.Now;
            UpdateLastUse(command.User.Name);

            bool wasEnqueued = _commandQueue.Enqueue(command);

            return new CommandProcessingResult(command, true, true, wasEnqueued);

        }
    }
}
