using System.Drawing;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    [Serializable]
    public class FixedStickDirectionInput : IControllerInput
    {
        private static readonly Dictionary<ControllerStickDirection, Point> _pointMap = new Dictionary<ControllerStickDirection, Point>()
        {
            { ControllerStickDirection.North, new Point(0, 100) },
            { ControllerStickDirection.South, new Point(0, -100) },
            { ControllerStickDirection.West, new Point(-100, 0) },
            { ControllerStickDirection.East, new Point(100, 0) },
            { ControllerStickDirection.NorthEast, new Point(100, 100) },
            { ControllerStickDirection.SouthEast, new Point(100, -100) },
            { ControllerStickDirection.NorthWest, new Point(-100, 100) },
            { ControllerStickDirection.SouthWest, new Point(-100, -100) }
        };

        public ControllerInputType InputType => ControllerInputType.Stick;

        public ControllerStickDirection StickDirection { get; set; }

        public ControllerStick Stick { get; set; } = ControllerStick.Left;

        public Point Position => _pointMap[StickDirection];

        public string GetMacro()
        {
            Point position = Position;
            ControllerStickInput controllerStickInput = new ControllerStickInput(Stick, position.X, position.Y);

            return controllerStickInput.GetMacro();

        }
    }
}
