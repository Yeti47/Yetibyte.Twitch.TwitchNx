using System.Drawing;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
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
            { ControllerStickDirection.NorthEast, new Point(071, 071) },
            { ControllerStickDirection.SouthEast, new Point(071, -071) },
            { ControllerStickDirection.NorthWest, new Point(-071, 071) },
            { ControllerStickDirection.SouthWest, new Point(-071, -071) }
        };

        [Newtonsoft.Json.JsonIgnore]
        public ControllerInputType InputType => ControllerInputType.Stick;

        public ControllerStickDirection StickDirection { get; set; }

        public float Pressure { get; set; } = 1f;

        public ControllerStick Stick { get; set; } = ControllerStick.Left;

        [Newtonsoft.Json.JsonIgnore]
        public Point Position
        {
            get
            {
                Point directionUnscaled = _pointMap[StickDirection];

                PointF directionScaled = new PointF(directionUnscaled.X * Pressure, directionUnscaled.Y * Pressure);

                return Point.Truncate(directionScaled);
            }
        }

        public string GetMacro()
        {
            Point position = Position;
            ControllerStickInput controllerStickInput = new ControllerStickInput(Stick, position.X, position.Y);

            return controllerStickInput.GetMacro();

        }
    }
}
