using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    internal class SameInputTypeMacroTimeTrackElementEqualityComparer : IEqualityComparer<MacroTimeTrackElement>
    {
        public bool Equals(MacroTimeTrackElement? x, MacroTimeTrackElement? y)
        {
            return object.ReferenceEquals(x?.Instruction, y?.Instruction) 
                || (   x?.Instruction != null && y?.Instruction != null 
                    && x.Instruction.ControllerInputType == y.Instruction.ControllerInputType 
                    && x.Instruction.Stick == y.Instruction.Stick 
                    && x.Instruction.Button == y.Instruction.Button
                   );
        }

        public int GetHashCode([DisallowNull] MacroTimeTrackElement obj)
        {
            return Tuple.Create(obj.Instruction.Button, obj.Instruction.ControllerInputType, obj.Instruction.Stick).GetHashCode();
        }
    }
}
