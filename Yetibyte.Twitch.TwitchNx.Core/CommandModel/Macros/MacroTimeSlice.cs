using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    public class MacroTimeSlice
    {
        private static readonly CultureInfo SECONDS_FORMAT_CULTURE = new CultureInfo("en-US");

        private readonly IControllerInput[] _controllerInputs;

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public TimeSpan Duration => EndTime - StartTime;

        public IEnumerable<IControllerInput> ControllerInputs => _controllerInputs;

        public MacroTimeSlice(TimeSpan startTime, TimeSpan endTime, IEnumerable<IControllerInput> controllerInputs)
        {
            StartTime = startTime;
            EndTime = endTime;

            _controllerInputs = controllerInputs.ToArray();
        }

        public string BuildMacro()
        {
            StringBuilder macroBuilder = new StringBuilder();

            foreach(var input in _controllerInputs)
            {
                macroBuilder.Append(input.GetMacro());
                macroBuilder.Append(' ');
            } 

            macroBuilder.Append(Duration.TotalSeconds.ToString("F4", SECONDS_FORMAT_CULTURE));
            macroBuilder.Append('s');

            return macroBuilder.ToString();
        }

    }
}
