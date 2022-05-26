using System.Collections;
using System.Globalization;
using System.Text;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class StickRotationMacroInstruction : IMacroInstruction
    {
        private static readonly CultureInfo SECONDS_FORMAT_CULTURE = new CultureInfo("en-US");

        [Newtonsoft.Json.JsonIgnore]
        public ControllerInputType InputType => ControllerInputType.Stick;

        public bool IsCounterClockwise { get; set; }

        [Newtonsoft.Json.JsonProperty]
        ControllerStick? IMacroInstruction.Stick => this.Stick;

        [Newtonsoft.Json.JsonProperty]
        public ControllerStick Stick { get; private set; } = ControllerStick.Left;

        [Newtonsoft.Json.JsonProperty]
        public ControllerStickDirection StartDirection { get; private set; }

        [Newtonsoft.Json.JsonProperty]
        public ControllerStickDirection EndDirection { get; private set; }

        [Newtonsoft.Json.JsonIgnore]
        public ControllerButton? Button => null;

        [Newtonsoft.Json.JsonIgnore]
        public int StepDelta
        {
            get
            {
                int delta = (int)EndDirection - (int)StartDirection;

                if (delta == 0)
                {
                    delta = (int)Enum.GetValues<ControllerStickDirection>().Max() + 1;
                }

                return delta * (IsCounterClockwise ? -1 : 1);
            }

        }

        [Newtonsoft.Json.JsonIgnore]
        public IEnumerable<ControllerStickDirection> Steps
        {
            get
            {
                int stepDelta = StepDelta;

                for (int i = 0; i <= Math.Abs(stepDelta); i++)
                {
                    int stepValue = ((int)StartDirection + i * Math.Sign(stepDelta)) % 8;

                    yield return (ControllerStickDirection)stepValue;
                }
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public MacroInstructionType InstructionType => MacroInstructionType.Animation;

        [Newtonsoft.Json.JsonIgnore]
        public ControllerInputType ControllerInputType => ControllerInputType.Stick;

        public StickRotationMacroInstruction(ControllerStickDirection startDirection, ControllerStickDirection endDirection, ControllerStick stick = ControllerStick.Left)
        {
            StartDirection = startDirection;
            EndDirection = endDirection;
            Stick = stick;
        }

        private StickRotationMacroInstruction()
        {

        }

        public IEnumerator<IControllerInput> GetEnumerator()
        {
            foreach (var stepDirectionInput in Steps.Select(s => new FixedStickDirectionInput { Stick = Stick, StickDirection = s }))
            {
                yield return stepDirectionInput;
            }
        }

        //public override string ToString()
        //{
        //    StringBuilder macroBuilder = new StringBuilder();
        //    double stepDuration = GetStepDuration();

        //    int i = 0;

        //    foreach (var stepDirectionInput in this)
        //    {
        //        if (i == 0 && Button is not null)
        //        {
        //            macroBuilder.Append(Button.GetMacro());
        //            macroBuilder.Append(" ");
        //        }

        //        macroBuilder.Append(stepDirectionInput.GetMacro());
        //        macroBuilder.Append(" ");
        //        macroBuilder.Append(stepDuration.ToString("F4", SECONDS_FORMAT_CULTURE));
        //        macroBuilder.Append("s");
        //        macroBuilder.Append("\n");

        //        i++;
        //    }

        //    return macroBuilder.ToString();
        //}

        private double GetStepDuration(TimeSpan totalDuration)
        {
            return Steps.Count() != 0
                            ? totalDuration.TotalSeconds / Steps.Count()
                            : 0;
        }

        public IEnumerable<TimeBoxedControllerInput> GetControllerInputs(TimeSpan parentStartTime, TimeSpan parentEndTime)
        {
            TimeSpan targetTotalDuration = parentEndTime - parentStartTime;
            TimeSpan totalDuration = TimeSpan.Zero;

            double stepDurationSeconds = GetStepDuration(targetTotalDuration);

            int i = 0;

            int stepCount = Steps.Count();

            foreach(var stepDirectionInput in this)
            {
                bool isLastStep = i == (stepCount - 1);

                TimeSpan stepDuration = TimeSpan.FromSeconds(stepDurationSeconds);

                totalDuration += stepDuration;

                TimeSpan relativeStartTime = i++ * stepDuration;

                if (isLastStep)
                {
                    stepDuration += (targetTotalDuration - totalDuration);
                }

                TimeSpan relativeEndTime = relativeStartTime + stepDuration;

                TimeSpan absoluteStartTime = parentStartTime + relativeStartTime;
                TimeSpan absoluteEndTime = absoluteStartTime + stepDuration;

                yield return new TimeBoxedControllerInput(stepDirectionInput, relativeStartTime, relativeEndTime, absoluteStartTime, absoluteEndTime);
            }
        }
    }
}
