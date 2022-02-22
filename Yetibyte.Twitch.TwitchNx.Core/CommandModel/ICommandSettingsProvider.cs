using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public interface ICommandSettingsProvider
    {
        CommandSettings CommandSettings { get; }
    }
}
