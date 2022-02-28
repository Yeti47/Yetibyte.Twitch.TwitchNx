using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class Macro
    {
        private const string VALIDATION_REGEX = @"^(\s*(([ABLRXY]|(Z[LR])|(DPAD_UP)|(DPAD_DOWN)|(DPAD_LEFT)|(DPAD_RIGHT)|(HOME)|(CAPTURE)|(PLUS)|(MINUS)|(JCL_SR)|(JCL_SL)|(JCR_SR)|(JCR_SL)|([LR]_STICK_PRESS)|([LR]_STICK@([+-][0-1][0-9][0-9]){2}))\s+)+\d(\.\d+)?s\s*(\n|$))*$";

        [Newtonsoft.Json.JsonIgnore]
        public double TotalSeconds => throw new NotImplementedException();

        public static bool Validate(string macroText)
        {
            return Regex.IsMatch(macroText, VALIDATION_REGEX);
        }
    }
}
