using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public interface ICommandSourceProvider
    {
        void Load();
        IEnumerable<ICommandSourceFactory> GetCommandSourceFactories();
    }
}
