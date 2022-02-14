using System.Drawing;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models
{
    public class SwitchController
    {
        private ControllerState _state = ControllerState.Initializing;

        public int Id { get; }
        public ControllerType ControllerType { get; }
        public ControllerState State
        {
            get => _state;
            set
            {
                _state = value;
                OnStateChanged();
            }
        }

        public string ErrorMessage { get; set; } = string.Empty;

        public Color BodyColor { get; init; } = Color.White;
        public Color ButtonColor { get; init; } = Color.Black;


        public event EventHandler<EventArgs>? StateChanged;


        public SwitchController(int id, ControllerType controllerType)
        {
            Id = id;
            ControllerType = controllerType;
        }

        protected virtual void OnStateChanged()
        {
            var handler = StateChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
