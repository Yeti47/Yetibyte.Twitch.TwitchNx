namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    public class KeyFrameMacroInstruction : IMacroInstruction
    {
        private sealed class EmptyControllerInput : IControllerInput
        {
            public static EmptyControllerInput Instance { get;  } = new EmptyControllerInput();

            public ControllerInputType InputType => ControllerInputType.Button;

            public string GetMacro() => string.Empty;

            private EmptyControllerInput() { }
        }

        public MacroInstructionType InstructionType => MacroInstructionType.Simple;

        public ControllerInputType ControllerInputType => ControllerInputType.Button;

        public ControllerStick? Stick => null;

        public ControllerButton? Button => null;

        public IEnumerable<TimeBoxedControllerInput> GetControllerInputs(TimeSpan parentStartTime, TimeSpan parentEndTime)
        {
            return new[] { new TimeBoxedControllerInput(
                EmptyControllerInput.Instance,
                TimeSpan.Zero,
                parentEndTime - parentStartTime,
                parentStartTime,
                parentEndTime)
             };
        }
    }
}
