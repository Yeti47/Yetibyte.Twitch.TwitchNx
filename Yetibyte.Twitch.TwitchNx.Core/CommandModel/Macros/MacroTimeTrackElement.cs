namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    public class MacroTimeTrackElement
    {
        [Newtonsoft.Json.JsonProperty]
        public string Id { get; private set; }

        [Newtonsoft.Json.JsonIgnore]
        public TimeSpan Duration => EndTime - StartTime;

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Newtonsoft.Json.JsonProperty]
        public IMacroInstruction Instruction { get; private set; }

        public MacroTimeTrackElement(string id, TimeSpan startTime, TimeSpan endTime, IMacroInstruction instruction)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            Instruction = instruction;
        }

        public IEnumerable<TimeBoxedControllerInput> GetControllerInputs() => Instruction.GetControllerInputs(StartTime, EndTime);

        public TimeBoxedControllerInput Slice(TimeSpan relativeStartTime, TimeSpan relativeEndTime)
        {
            IEnumerable<TimeBoxedControllerInput> controllerInputs = GetControllerInputs();

            TimeBoxedControllerInput sourceInput = controllerInputs
                .OrderBy(ti => ti.StartTime)
                .Last(ti => ti.StartTime <= relativeStartTime);

            TimeBoxedControllerInput slicedInput = new TimeBoxedControllerInput(
                sourceInput.ControllerInput,
                relativeStartTime,
                new[] { relativeEndTime, sourceInput.EndTime }.Min(),
                relativeStartTime + StartTime,
                relativeEndTime + EndTime
            );

            return slicedInput;
        }


    }
}
