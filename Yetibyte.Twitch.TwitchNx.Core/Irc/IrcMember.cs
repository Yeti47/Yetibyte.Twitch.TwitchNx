using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing;

namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{
    public class IrcMember : ICommandUser
    {
        public string Name { get; }
        public string DisplayName { get; }

        public Color Color { get; init; } = Color.White;

        public bool IsMod { get; init; } = false;
        public bool IsSubscriber { get; init; } = false;
        public bool IsMe { get; init; } = false;
        public bool IsOwner { get; init; } = false;

        public IrcMember(string name, string displayName = "")
        {
            Name = name;
            DisplayName = !string.IsNullOrWhiteSpace(displayName) ? displayName : name;
        }

        public override string ToString() => Name;

    }
}
