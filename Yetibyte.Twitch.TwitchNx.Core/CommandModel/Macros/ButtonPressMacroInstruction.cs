namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class ButtonPressMacroInstruction : IMacroInstruction
    {
        [Newtonsoft.Json.JsonProperty("ControllerButton")]
        private ControllerButton _controllerButton;

        [Newtonsoft.Json.JsonIgnore]
        public MacroInstructionType InstructionType => MacroInstructionType.Simple;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerInputType ControllerInputType => ControllerInputType.Button;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerStick? Stick => null;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerButton? Button => _controllerButton;

        private ButtonPressMacroInstruction()
        {
            _controllerButton = ControllerButton.A;
        }

        public ButtonPressMacroInstruction(ControllerButton controllerButton)
        {
            _controllerButton = controllerButton;
        }

        public IEnumerable<TimeBoxedControllerInput> GetControllerInputs(TimeSpan parentStartTime, TimeSpan parentEndTime)
        {
            yield return new TimeBoxedControllerInput(_controllerButton, TimeSpan.Zero, parentEndTime - parentStartTime, parentStartTime, parentEndTime);
        }
    }
}
