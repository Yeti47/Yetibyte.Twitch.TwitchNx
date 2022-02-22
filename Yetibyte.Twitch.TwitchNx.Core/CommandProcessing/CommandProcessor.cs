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
        private readonly Dictionary<string, DateTime> _lastUses = new Dictionary<string, DateTime>();

        private readonly CommandSetup _commandSetup;
        private readonly ICommandQueue _commandQueue;

        private DateTime _sharedLastUse;
    }
}
