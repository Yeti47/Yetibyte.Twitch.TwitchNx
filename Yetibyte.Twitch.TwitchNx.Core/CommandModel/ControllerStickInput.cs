namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class ControllerStickInput : IControllerInput
    {
        public ControllerInputType InputType => ControllerInputType.Stick;

        public ControllerStick Stick { get; }

        public string StickMacro => Stick == ControllerStick.Left ? "L_STICK" : "R_STICK";

        public string MacroX => X > 0 ? $"+{X:000}" : $"-{X:000}";
        public string MacroY => Y > 0 ? $"+{Y:000}" : $"-{Y:000}";

        public int X { get; }
        public int Y { get; }

        public ControllerStickInput(ControllerStick stick, int x, int y)
        {
            Stick = stick;
            X = Math.Max(0, Math.Min(x, 100));
            Y = Math.Max(0, Math.Min(y, 100));
        }

        public string Macro => $"{StickMacro}@{MacroX}{MacroY}";
    }
}
