using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public class Command : IEquatable<Command>
    {
        public string Id { get; } 

        public ICommandUser User { get; }

        public string OriginalMessage { get; }

        public CommandSetup CommandSetup { get; }

        public Command(string id, ICommandUser user, CommandSetup commandSetup, string originalMessage)
        {
            Id = id;
            User = user;
            CommandSetup = commandSetup;
            OriginalMessage = originalMessage;
        }

        public override bool Equals(object? obj) => ReferenceEquals(obj, this) || obj is Command other && other.Id == Id;
        public bool Equals(Command? other) => ReferenceEquals(other, this) || other is Command otherCmd && otherCmd.Id == Id;

        public override int GetHashCode() => Id.GetHashCode();

    }
}
