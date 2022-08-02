using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class Macro
    {
        private const string TIME_NUMERIC_REGEX = @"\d+(\.\d+)?";

        private const string VALIDATION_REGEX = @"^(\s*(([ABLRXY]|(Z[LR])|(DPAD_UP)|(DPAD_DOWN)|(DPAD_LEFT)|(DPAD_RIGHT)|(HOME)|(CAPTURE)|(PLUS)|(MINUS)|(JCL_SR)|(JCL_SL)|(JCR_SR)|(JCR_SL)|([LR]_STICK_PRESS)|([LR]_STICK@([+-][0-1][0-9][0-9]){2}))\s+)+\d(\.\d+)?s\s*(\n|$))*$";
        private const int TIME_SLICE_MIN_TICKS = 5;

        [Newtonsoft.Json.JsonProperty("TimeTracks")]
        private List<MacroTimeTrack> _timeTracks = new List<MacroTimeTrack>();

        [Newtonsoft.Json.JsonIgnore]
        public double TotalSeconds => !_timeTracks.Any() ? 0 : _timeTracks.Max(t => t.GetTotalDuration().TotalSeconds);

        [Newtonsoft.Json.JsonIgnore]
        public IEnumerable<MacroTimeTrack> TimeTracks => new ReadOnlyCollection<MacroTimeTrack>(_timeTracks);

        [Newtonsoft.Json.JsonIgnore]
        public int TimeTrackCount => _timeTracks.Count;

        public static bool Validate(string macroText)
        {
            return Regex.IsMatch(macroText, VALIDATION_REGEX);
        }

        public static IEnumerable<string> GetLines(string macroText) => macroText.Replace("\r", string.Empty).Split("\n");

        public static float GetMacroDuration(string macroText)
        {
            IEnumerable<string> macroLines = GetLines(macroText);

            CultureInfo formatCulture = new CultureInfo("en-US");

            return macroLines.SelectMany(ml => Regex.Matches(ml, TIME_NUMERIC_REGEX)).Sum(m => Convert.ToSingle(m.Value, formatCulture));
        }

        public void Clear() => _timeTracks.Clear();

        public bool ContainsTimeTrack(MacroTimeTrack timeTrack) => _timeTracks.Contains(timeTrack);

        public bool AddTimeTrack(MacroTimeTrack timeTrack)
        {
            if (ContainsTimeTrack(timeTrack))
                return false;

            _timeTracks.Add(timeTrack);

            return true;
        }

        public MacroTimeTrack AddNewTimeTrack()
        {
            MacroTimeTrack timeTrack = new MacroTimeTrack();

            _timeTracks.Add(timeTrack);

            return timeTrack;
        }

        public bool RemoveTimeTrack(MacroTimeTrack macroTimeTrack)
        {
            if (!ContainsTimeTrack(macroTimeTrack))
                return false;

            return _timeTracks.Remove(macroTimeTrack);
        }

        public IEnumerable<MacroTimeSlice> FindTimeSlices()
        {
            TimeSpan[] timeStamps = GetTimeStamps().ToArray();

            for (int i = 0; i < timeStamps.Length - 1; i++)
            {
                TimeSpan startTime = timeStamps[i];
                TimeSpan endTime = timeStamps[i + 1];

                if (Math.Abs(endTime.Ticks - startTime.Ticks) < TIME_SLICE_MIN_TICKS)
                    continue;

                IEnumerable<MacroTimeTrackElement> overlappingElements = GetElementsBetween(startTime, endTime);

                IEnumerable<TimeBoxedControllerInput> timeBoxedControllerInputs = overlappingElements.Select(e => e.Slice(startTime - e.StartTime, endTime - e.StartTime));   

                IEnumerable<IControllerInput> controllerInputs = timeBoxedControllerInputs.Select(tci => tci.ControllerInput);

                yield return new MacroTimeSlice(startTime, endTime, controllerInputs);
            }
        }

        public IEnumerable<MacroTimeTrackElement> GetElementsBetween(TimeSpan startTime, TimeSpan endTime)
        {
            return _timeTracks.SelectMany(t => t).OrderBy(e => e.StartTime).Where(e =>
                     startTime < e.EndTime 
                  && e.StartTime < endTime
            ).Distinct(new SameInputTypeMacroTimeTrackElementEqualityComparer());
        }

        public IOrderedEnumerable<TimeSpan> GetTimeStamps() =>
            new [] { TimeSpan.Zero }.Concat(
                _timeTracks
                .SelectMany(t => t)
                .SelectMany(e => e.GetControllerInputs())
                .Select(e => e.AbsoluteStartTime)
                .Distinct()
                .Concat(_timeTracks.SelectMany(t => t).SelectMany(e => e.GetControllerInputs()).Select(e => e.AbsoluteEndTime).Distinct())
            ).OrderBy(t => t.Ticks);

        public string Build()
        {
            StringBuilder macroStringBuilder = new StringBuilder();

            foreach(var timeSlice in FindTimeSlices())
            {
                macroStringBuilder.Append(timeSlice.BuildMacro());
                macroStringBuilder.Append('\n');
            }

            return macroStringBuilder.ToString();
        }

    }
}
