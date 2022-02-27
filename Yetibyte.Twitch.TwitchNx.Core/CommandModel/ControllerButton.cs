namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public record ControllerButton(string Name, string Macro) : IControllerInput
    {
        public static ControllerButton A { get; } = new ControllerButton("A", "A");
        public static ControllerButton B { get; } = new ControllerButton("B", "B");
        public static ControllerButton X { get; } = new ControllerButton("X", "X");
        public static ControllerButton Y { get; } = new ControllerButton("Y", "Y");

        public static ControllerButton L { get; } = new ControllerButton("L", "L");
        public static ControllerButton ZL { get; } = new ControllerButton("ZL", "ZL");
        public static ControllerButton R { get; } = new ControllerButton("R", "R");
        public static ControllerButton ZR { get; } = new ControllerButton("ZR", "R");

        public static ControllerButton Minus { get; } = new ControllerButton("Minus", "MINUS");
        public static ControllerButton Plus { get; } = new ControllerButton("Plus", "PLUS");

        public static ControllerButton Home { get; } = new ControllerButton("Home", "Home");
        public static ControllerButton Capture { get; } = new ControllerButton("Capture", "CAPTURE");

        public static ControllerButton RightStickPress { get; } = new ControllerButton("Right Stick Press", "R_STICK_PRESS");
        public static ControllerButton LeftStickPress { get; } = new ControllerButton("Left Stick Press", "L_STICK_PRESS");

        public static ControllerButton JoyConLeftSr { get; } = new ControllerButton("JoyCon Left SR", "JCL_SR");
        public static ControllerButton JoyConRightSr { get; } = new ControllerButton("JoyCon Right SR", "JCR_SR");
        public static ControllerButton JoyConLeftSl { get; } = new ControllerButton("JoyCon Left SL", "JCL_SL");
        public static ControllerButton JoyConRightSl { get; } = new ControllerButton("JoyCon RIGHT SL", "JCR_SL");

        public static ControllerButton DpadUp { get; } = new ControllerButton("D-Pad Up", "DPAD_UP");
        public static ControllerButton DpadDown { get; } = new ControllerButton("D-Pad Down", "DPAD_DOWN");
        public static ControllerButton DpadLeft { get; } = new ControllerButton("D-Pad Left", "DPAD_LEFT");
        public static ControllerButton DpadRight { get; } = new ControllerButton("D-Pad Right", "DPAD_RIGHT");

        public ControllerInputType InputType => ControllerInputType.Button;

        public override string ToString() => Name;

        public string GetMacro() => Macro;
    }
}
