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
    public class SwitchControlViewModel : ToolViewModel, ISwitchControllerSelector
    {
        private readonly SwitchConnector _switchConnector;
        private readonly ObservableCollection<ControllerViewModel> _controllers = new ObservableCollection<ControllerViewModel>();
        private readonly RelayCommand _addControllerCommand;
        private readonly RelayCommand _removeControllerCommand;
        private readonly RelayCommand<int> _removeControllerAtCommand;
        private readonly RelayCommand _unselectCommand;

        private ControllerViewModel? _selectedController;

        private bool _isProControllerChecked = true;
        private bool _isJoyConLeftChecked = false;
        private bool _isJoyConRightChecked = false;

        public string SwitchAddress => _switchConnector.SwitchAddress;

        public ICommand AddControllerCommand => _addControllerCommand;
        public ICommand RemoveControllerCommand => _removeControllerCommand;
        public ICommand RemoveControllerAtCommand => _removeControllerAtCommand;
        public ICommand UnselectCommand => _unselectCommand;

        public IEnumerable<ControllerViewModel> Controllers => _controllers;

        public ControllerViewModel? SelectedController
        {
            get => _selectedController;
            set
            {
                _selectedController = value;
                OnPropertyChanged();
                _unselectCommand.NotifyCanExecuteChanged();
                _removeControllerCommand.NotifyCanExecuteChanged();

                OnSelectedControllerChanged();
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

        SwitchController? ISwitchControllerSelector.SelectedController => _switchConnector.Controllers.FirstOrDefault(c => c.Id == SelectedController?.Id);

        bool ISwitchControllerSelector.HasSelectedController => SelectedController is not null;

        public event EventHandler? SelectedControllerChanged;

        public SwitchControlViewModel(SwitchConnector switchConnector) : base("Switch Control Panel")
        {
            _switchConnector = switchConnector;

            _switchConnector.Connected += switchConnector_Connected;
            _switchConnector.Disconnected += switchConnector_Disconnected;
            _switchConnector.StatusUpdated += switchConnector_StatusUpdated;
            _switchConnector.SwitchAddressChanged += switchConnector_SwitchAddressChanged;
            _switchConnector.ControllerAdded += _switchConnector_ControllerAdded;
            _switchConnector.ControllerRemoved += _switchConnector_ControllerRemoved;

            _addControllerCommand = new RelayCommand(ExecuteAddControllerCommand, CanExecuteAddControllerCommand);
            _removeControllerCommand = new RelayCommand(ExecuteRemoveControllerCommand, CanExecuteRemoveControllerCommand);
            _removeControllerAtCommand = new RelayCommand<int>(ExecuteRemoveControllerAtCommand, CanExecuteRemoveControllerAtCommand);
            _unselectCommand = new RelayCommand(() => SelectedController = null, () => SelectedController != null);
        }

        private void ExecuteRemoveControllerAtCommand(int id)
        {
            if (!CanExecuteRemoveControllerAtCommand(id))
                return;

            _switchConnector.RemoveController(id);
        }

        private bool CanExecuteRemoveControllerAtCommand(int id)
        {
            return _controllers.Any(c => c.Id == id) && _switchConnector.IsConnected;
        }

        private void switchConnector_SwitchAddressChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SwitchAddress));
        }

        private void NotifyControllerCommandsCanExecuteChanged()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                _addControllerCommand.NotifyCanExecuteChanged();
                _removeControllerCommand.NotifyCanExecuteChanged();

                foreach(var controller in _controllers)
                    controller.NotifyRemoveCommandCanExecuteChanged();
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

        private void ExecuteRemoveControllerCommand()
        {
            if (SelectedController is null)
                return;

            _switchConnector.RemoveController(SelectedController.Id);
        }

        private bool CanExecuteRemoveControllerCommand()
        {
            return SelectedController is not null && _switchConnector.IsConnected;
        }

        private void _switchConnector_ControllerRemoved(object? sender, SwitchControllerRemovedEventArgs e)
        {
            var controllerViewModel = FindViewModelForController(e.ControllerId);

            if (controllerViewModel is not null)
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        _controllers.Remove(controllerViewModel);
                    }
                );
            }

        }

        private void _switchConnector_ControllerAdded(object? sender, SwitchControllerAddedEventArgs e)
        {
            var controllerViewModel = FindViewModelForController(e.Controller);

            if (controllerViewModel is null)
            {
                controllerViewModel = new ControllerViewModel(_switchConnector);

                Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        _controllers.Add(controllerViewModel);
                    }
                );
            }

            controllerViewModel.Update(e.Controller);
        }

        private void switchConnector_StatusUpdated(object? sender, EventArgs e)
        {
            foreach(ControllerViewModel existingControllerVm in _controllers)
            {
                if (!_switchConnector.Controllers.Any(c => c.Id == existingControllerVm.Id)) // Controller in VM not found in actual Switch, so remove from VM
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            _controllers.Remove(existingControllerVm);
                        }
                    );
                }
            }

            foreach(SwitchController controller in _switchConnector.Controllers)
            {
                var controllerViewModel = FindViewModelForController(controller);

                if (controllerViewModel is null) // Controller not found in view model, so create it
                {
                    controllerViewModel = new ControllerViewModel(_switchConnector);
                    controllerViewModel.Update(controller);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        _controllers.Add(controllerViewModel);
                    }
                    );
                }

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

        protected virtual void OnSelectedControllerChanged()
        {
            var handler = SelectedControllerChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
