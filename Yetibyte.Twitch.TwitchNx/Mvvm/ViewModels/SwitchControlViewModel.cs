using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class SwitchControlViewModel : ToolViewModel
    {
        private readonly SwitchConnector _switchConnector;
        private readonly ObservableCollection<ControllerViewModel> _controllers = new ObservableCollection<ControllerViewModel>();
        private readonly RelayCommand _addControllerCommand;

        private ControllerViewModel? _selectedController;

        private bool _isProControllerChecked = true;
        private bool _isJoyConLeftChecked = false;
        private bool _isJoyConRightChecked = false;

        public string SwitchAddress => _switchConnector.SwitchAddress;

        public ICommand AddControllerCommand => _addControllerCommand;

        public IEnumerable<ControllerViewModel> Controllers => _controllers;

        public ControllerViewModel? SelectedController
        {
            get => _selectedController;
            set
            {
                _selectedController = value;
                OnPropertyChanged();
            }
        }

        public bool IsProControllerChecked
        {
            get { return _isProControllerChecked; }
            set { _isProControllerChecked = value; OnPropertyChanged(); }
        }

        public bool IsJoyConLeftChecked
        {
            get { return _isJoyConLeftChecked; }
            set { _isJoyConLeftChecked = value; OnPropertyChanged(); }
        }

        public bool IsJoyConRightChecked
        {
            get { return _isJoyConRightChecked; }
            set { _isJoyConRightChecked = value; OnPropertyChanged(); }
        }

        public SwitchControlViewModel(SwitchConnector switchConnector) : base("Switch Control Panel")
        {
            _switchConnector = switchConnector;

            _switchConnector.Connected += switchConnector_Connected;
            _switchConnector.Disconnected += switchConnector_Disconnected;
            _switchConnector.StatusUpdated += switchConnector_StatusUpdated;
            _switchConnector.ControllerAdded += _switchConnector_ControllerAdded;
            _switchConnector.ControllerRemoved += _switchConnector_ControllerRemoved;

            _addControllerCommand = new RelayCommand(ExecuteAddControllerCommand, CanExecuteAddControllerCommand);
        }

        private void NotifyControllerCommandsCanExecuteChanged()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                _addControllerCommand.NotifyCanExecuteChanged();
            });
        }

        private void switchConnector_Disconnected(object? sender, EventArgs e)
        {
            NotifyControllerCommandsCanExecuteChanged();
        }

        private void switchConnector_Connected(object? sender, EventArgs e)
        {
            NotifyControllerCommandsCanExecuteChanged();
        }

        private void ExecuteAddControllerCommand()
        {
            ControllerType controllerType =
                IsJoyConLeftChecked
                ? ControllerType.JoyConLeft 
                : (IsJoyConRightChecked ? ControllerType.JoyConRight : ControllerType.ProController);

            _switchConnector.AddController(controllerType);
        }

        private bool CanExecuteAddControllerCommand()
        {
            return _switchConnector.IsConnected;
        }

        private void _switchConnector_ControllerRemoved(object? sender, SwitchControllerRemovedEventArgs e)
        {
            var controllerViewModel = FindViewModelForController(e.ControllerId);

            if (controllerViewModel is not null)
            {
                _controllers.Remove(controllerViewModel);
            }

        }

        private void _switchConnector_ControllerAdded(object? sender, SwitchControllerAddedEventArgs e)
        {
            var controllerViewModel = FindViewModelForController(e.Controller);

            if (controllerViewModel is null)
            {
                controllerViewModel = new ControllerViewModel();
                _controllers.Add(controllerViewModel);
            }

            controllerViewModel.Update(e.Controller);
        }

        private void switchConnector_StatusUpdated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SwitchAddress));

            foreach(SwitchController controller in _switchConnector.Controllers)
            {
                var controllerViewModel = FindViewModelForController(controller);

                if (controllerViewModel is null)
                    continue;

                controllerViewModel.Update(controller); 
            }
        }

        private ControllerViewModel? FindViewModelForController(SwitchController switchController)
        {
            return _controllers.FirstOrDefault(c => c.Id == switchController.Id);
        }

        private ControllerViewModel? FindViewModelForController(int controllerId)
        {
            return _controllers.FirstOrDefault(c => c.Id == controllerId);
        }
    }
}
