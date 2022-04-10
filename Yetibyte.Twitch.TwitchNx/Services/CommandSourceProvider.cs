using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    public class CommandSourceProvider : ICommandSourceProvider
    {
        private readonly List<ICommandSourceFactory> _factories = new List<ICommandSourceFactory>();

        public CommandSourceProvider(IEnumerable<ICommandSourceFactory> commandSourceFactories)
        {
            _factories.AddRange(commandSourceFactories);
        }

        public IEnumerable<ICommandSourceFactory> GetCommandSourceFactories()
        {
            return _factories;
        }
    }
}
