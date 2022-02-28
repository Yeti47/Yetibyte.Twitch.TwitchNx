using System.Collections;
using System.Globalization;
using System.Text;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class StickRotationMacroInstruction : IMacroInstruction
    {
        private static readonly CultureInfo SECONDS_FORMAT_CULTURE = new CultureInfo("en-US");

        public ControllerInputType InputType => ControllerInputType.Stick;

        public bool IsCounterClockwise { get; set; }

        public ControllerStick Stick { get; private set; } = ControllerStick.Left;

        public ControllerStickDirection StartDirection { get; private set; }
        public ControllerStickDirection EndDirection { get; private set; }

        public ControllerButton? Button { get; set; }

        public int StepDelta
        {
            get
            {
                int delta = (int)EndDirection - (int)StartDirection;

                if (delta == 0)
                {
                    delta = 8;
                }

                return delta * (IsCounterClockwise ? -1 : 1);
            }

        }

        public IEnumerable<ControllerStickDirection> Steps
        {
            get
            {
                int stepDelta = StepDelta;

                for (int i = 0; i < Math.Abs(stepDelta); i++)
                {
                    int stepValue = ((int)StartDirection + i * Math.Sign(stepDelta)) % 8;

                    yield return (ControllerStickDirection)stepValue;
                }
            }
        }

        public float Seconds { get; set; }

        public MacroInstructionType InstructionType => MacroInstructionType.Animation;

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


        public override string ToString()
        {
            StringBuilder macroBuilder = new StringBuilder();

            double stepDuration = Steps.Count() != 0
                ? Seconds / Steps.Count()
                : 0;

            int i = 0;

            foreach (var stepDirectionInput in this)
            {
                if (i == 0 && Button is not null)
                {
                    macroBuilder.Append(Button.GetMacro());
                    macroBuilder.Append(" ");
                }

                macroBuilder.Append(stepDirectionInput.GetMacro());
                macroBuilder.Append(" ");
                macroBuilder.Append(stepDuration.ToString(".4", SECONDS_FORMAT_CULTURE));
                macroBuilder.Append("s");
                macroBuilder.Append("\n");

                i++;
            }

            return macroBuilder.ToString();
        }

        public IEnumerable<TimeBoxedControllerInput> GetControllerInputs(TimeSpan parentStartTime, TimeSpan parentEndTime)
        {
            throw new NotImplementedException();
        }
    }
}
