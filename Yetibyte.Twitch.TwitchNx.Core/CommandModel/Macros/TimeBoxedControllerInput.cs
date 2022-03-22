using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public record TimeBoxedControllerInput(
        IControllerInput ControllerInput,  
        TimeSpan StartTime,
        TimeSpan EndTime,
        TimeSpan AbsoluteStartTime,
        TimeSpan AbsoluteEndTime
    );
}
