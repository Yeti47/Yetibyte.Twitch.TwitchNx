using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class MacroTesterViewModel : ToolViewModel
    {
        private const string ERROR_MESSAGE = "The macro you entered is invalid. Please check your input.";

        private readonly RelayCommand _sendMacroCommand;
        private readonly SwitchConnector _switchConnector;

        private string _macroText = string.Empty;
        private string _errorMessage = string.Empty;

        public SwitchControlViewModel SwitchControlViewModel { get; }

        public ICommand SendMacroCommand => _sendMacroCommand;

        public string MacroText
        {
            get { return _macroText; }
            set { 
                _macroText = value; 
                OnPropertyChanged(); 
                _sendMacroCommand.NotifyCanExecuteChanged();
                ErrorMessage = string.Empty;
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public MacroTesterViewModel(SwitchConnector switchConnector, SwitchControlViewModel switchControlViewModel) : base("Macro Tester")
        {
            _switchConnector = switchConnector;
            _switchConnector.Connected += switchConnector_Connected;
            _switchConnector.Disconnected += switchConnector_Disconnected;
            _switchConnector.ControllerAdded += switchConnector_ControllerAdded;
            _switchConnector.ControllerRemoved += switchConnector_ControllerRemoved;

            SwitchControlViewModel = switchControlViewModel;
            SwitchControlViewModel.PropertyChanged += SwitchControlViewModel_PropertyChanged;

            _sendMacroCommand = new RelayCommand(() =>
            {
                if (SwitchControlViewModel.SelectedController is not null && _switchConnector.IsConnected && !string.IsNullOrWhiteSpace(MacroText))
                {
                    string macroText = MacroText.Replace("\r", "");

                    if (!Macro.Validate(macroText))
                    {
                        ErrorMessage = ERROR_MESSAGE;
                        return;
                    }
                    else
                    {
                        ErrorMessage = string.Empty;
                    }

                    _switchConnector.ExecuteMacro(macroText, SwitchControlViewModel.SelectedController.Id);
                }
            }, () =>
            {
                return SwitchControlViewModel.SelectedController is not null && _switchConnector.IsConnected && !string.IsNullOrWhiteSpace(MacroText);
            });
        }

        private void SwitchControlViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SwitchControlViewModel.SelectedController))
            {
                RaiseCommandCanExecuteChanged();
            }
        }

        private void switchConnector_ControllerRemoved(object? sender, SwitchControllerRemovedEventArgs e)
        {
            RaiseCommandCanExecuteChanged();
        }

        private void switchConnector_ControllerAdded(object? sender, SwitchControllerAddedEventArgs e)
        {
            RaiseCommandCanExecuteChanged();
        }

        private void switchConnector_Disconnected(object? sender, EventArgs e)
        {
            RaiseCommandCanExecuteChanged();
        }

        private void switchConnector_Connected(object? sender, EventArgs e)
        {
            RaiseCommandCanExecuteChanged();
        }

        private void RaiseCommandCanExecuteChanged()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                _sendMacroCommand.NotifyCanExecuteChanged();
            });
        }
    }
}
