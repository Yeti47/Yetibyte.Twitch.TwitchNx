namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class FixedStickDirectionMacroInstruction : IMacroInstruction
    {
        [Newtonsoft.Json.JsonIgnore]
        public MacroInstructionType InstructionType => MacroInstructionType.Simple;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerInputType ControllerInputType => ControllerInputType.Stick;

        [Newtonsoft.Json.JsonIgnore]
        ControllerStick? IMacroInstruction.Stick => FixedStickDirectionInput.Stick;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerStick Stick => FixedStickDirectionInput.Stick;

        public FixedStickDirectionInput FixedStickDirectionInput { get; set; }

        public FixedStickDirectionMacroInstruction(FixedStickDirectionInput fixedStickDirectionInput)
        {
            FixedStickDirectionInput = fixedStickDirectionInput;
        }

        private FixedStickDirectionMacroInstruction()
        {
            FixedStickDirectionInput = new FixedStickDirectionInput();
        }

        public FixedStickDirectionMacroInstruction(ControllerStick stick, ControllerStickDirection stickDirection)
        {
            FixedStickDirectionInput = new FixedStickDirectionInput { Stick = stick, StickDirection = stickDirection };
        }   


        [Newtonsoft.Json.JsonIgnore]
        public ControllerButton? Button => null;

        public IEnumerable<TimeBoxedControllerInput> GetControllerInputs(TimeSpan parentStartTime, TimeSpan parentEndTime)
        {
            yield return new TimeBoxedControllerInput(FixedStickDirectionInput, TimeSpan.Zero, parentEndTime - parentStartTime, parentStartTime, parentEndTime);
        }
    }
}
