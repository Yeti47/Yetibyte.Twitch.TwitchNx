namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class ControllerStickInput : IControllerInput
    {
        [Newtonsoft.Json.JsonIgnore]
        public ControllerInputType InputType => ControllerInputType.Stick;

        [Newtonsoft.Json.JsonProperty]
        public ControllerStick Stick { get; private set; }

        [Newtonsoft.Json.JsonIgnore]
        public string StickMacro => Stick == ControllerStick.Left ? "L_STICK" : "R_STICK";

        [Newtonsoft.Json.JsonIgnore]
        public string MacroX => X >= 0 ? $"+{X:000}" : $"{X:000}";

        [Newtonsoft.Json.JsonIgnore]
        public string MacroY => Y >= 0 ? $"+{Y:000}" : $"{Y:000}";

        [Newtonsoft.Json.JsonProperty]
        public int X { get; private set; }

        [Newtonsoft.Json.JsonProperty]
        public int Y { get; private set; }

        public ControllerStickInput(ControllerStick stick, int x, int y)
        {
            Stick = stick;
            X = Math.Max(-100, Math.Min(x, 100));
            Y = Math.Max(-100, Math.Min(y, 100));
        }

        public string GetMacro()
        {
            return $"{StickMacro}@{MacroX}{MacroY}";
        }
    }
}
