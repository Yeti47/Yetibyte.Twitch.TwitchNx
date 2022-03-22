using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    public interface IMacroInstruction
    {
        MacroInstructionType InstructionType { get; }

        ControllerInputType ControllerInputType { get; }

        ControllerStick Stick { get; }
        ControllerButton? Button { get; }

        IEnumerable<TimeBoxedControllerInput> GetControllerInputs(TimeSpan parentStartTime, TimeSpan parentEndTime);
    }
}
