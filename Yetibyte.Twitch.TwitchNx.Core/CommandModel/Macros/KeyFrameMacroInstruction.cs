namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class KeyFrameMacroInstruction : IMacroInstruction
    {
        [Serializable]
        private sealed class EmptyControllerInput : IControllerInput
        {
            public static EmptyControllerInput Instance { get;  } = new EmptyControllerInput();

            [Newtonsoft.Json.JsonIgnore]
            public ControllerInputType InputType => ControllerInputType.Button;

            public string GetMacro() => string.Empty;

            private EmptyControllerInput() { }
        }

        [Newtonsoft.Json.JsonIgnore]
        public MacroInstructionType InstructionType => MacroInstructionType.Simple;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerInputType ControllerInputType => ControllerInputType.Button;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerStick? Stick => null;

        [Newtonsoft.Json.JsonIgnore]
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
