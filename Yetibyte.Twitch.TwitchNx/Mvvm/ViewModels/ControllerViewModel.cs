using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class ControllerViewModel : ObservableObject
    {
        private int _id;
        private ControllerType _controllerType;
        private ControllerState _state;
        private Color _bodyColor;
        private Color _color;

        public Color ButtonColor
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged(); }
        }

        public Color BodyColor
        {
            get { return _bodyColor; }
            set { _bodyColor = value; OnPropertyChanged(); }
        }

        public ControllerState State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }

        public ControllerType ControllerType
        {
            get { return _controllerType; }
            set { _controllerType = value; OnPropertyChanged(); }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public ControllerViewModel()
        {

        }

        public void Update(SwitchController switchController)
        {
            Id = switchController.Id;
            ControllerType = switchController.ControllerType;
            State = switchController.State;
            BodyColor = switchController.BodyColor;
            ButtonColor = switchController.ButtonColor;
        }

    }
}
