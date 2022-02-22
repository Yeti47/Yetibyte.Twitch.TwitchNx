using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class ControllerViewModel : ObservableObject
    {
        private readonly RelayCommand _removeCommand;
        private readonly SwitchConnector _switchConnector;
        private int _id;
        private ControllerType _controllerType;
        private ControllerState _state;
        private Color _bodyColor;
        private Color _color;
        private string _errorMessage = string.Empty;

        public ICommand RemoveCommand => _removeCommand;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { 
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsError));
            }
        }

        public bool IsConnecting => State == ControllerState.Connecting;

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
            set { 
                _state = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(IsError));
                OnPropertyChanged(nameof(IsConnecting));
            }
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

        public bool IsError => State == ControllerState.Crashed || !string.IsNullOrWhiteSpace(_errorMessage);

        public ControllerViewModel(SwitchConnector switchConnector)
        {
            _switchConnector = switchConnector;
            _removeCommand = new RelayCommand(
                () => _switchConnector.RemoveController(Id), 
                () => _switchConnector.IsConnected
            );

        }

        public void NotifyRemoveCommandCanExecuteChanged()
        {
            _removeCommand.NotifyCanExecuteChanged();
        }

        public void Update(SwitchController switchController)
        {
            Id = switchController.Id;
            ControllerType = switchController.ControllerType;
            State = switchController.State;
            BodyColor = switchController.BodyColor;
            ButtonColor = switchController.ButtonColor;
            ErrorMessage = switchController.ErrorMessage;
        }

    }
}
