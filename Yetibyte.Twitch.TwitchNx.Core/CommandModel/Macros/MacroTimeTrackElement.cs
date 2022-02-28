namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    public class MacroTimeTrackElement
    {
        public string Id { get; private set; }

        public TimeSpan Duration => EndTime - StartTime;

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

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
            TimeBoxedControllerInput sourceInput = GetControllerInputs()
                .OrderBy(ti => ti.StartTime)
                .First(ti => ti.StartTime <= relativeStartTime);

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
