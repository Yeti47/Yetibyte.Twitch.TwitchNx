using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class SwitchConnectionViewModel : ToolViewModel
    {
        #region Fields

        private readonly SwitchConnector _switchConnector;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _disconnectCommand;

        private string _clientAddress = string.Empty;
        private int _clientPort = SwitchBridgeClientConnectionSettings.DEFAULT_PORT;

        #endregion

        #region Props

        public ICommand ConnectCommand => _connectCommand;
        public ICommand DisconnectCommand => _disconnectCommand;

        public string ClientAddress
        {
            get => _clientAddress;
            set
            {
                if (_clientAddress != value)
                {
                    _clientAddress = value;
                    OnPropertyChanged();
                    NotifyConnectionStatePropsChanged();
                }
            }
        }

        public int ClientPort
        {
            get => _clientPort;
            set
            {
                if (_clientPort != value)
                {
                    _clientPort = value;
                    OnPropertyChanged();
                    NotifyConnectionStatePropsChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return ConnectionState == SwitchConnectionState.ConnectingToBridge
                    || (ConnectionState == SwitchConnectionState.ConnectedToBridge && !_switchConnector.IsConnectedToSwitch);
            }
        }

        public bool IsWaitingForSwitch => (ConnectionState == SwitchConnectionState.ConnectedToBridge && !_switchConnector.IsConnectedToSwitch);

        public bool CanConnect => _switchConnector.State == SwitchConnectorState.Disconnected;

        public SwitchConnectionState ConnectionState
        {
            get
            {
                return _switchConnector.State switch
                {
                    SwitchConnectorState.Connecting => SwitchConnectionState.ConnectingToBridge,
                    SwitchConnectorState.Connected => _switchConnector.IsConnectedToSwitch ? SwitchConnectionState.ConnectedToSwitch : SwitchConnectionState.ConnectedToBridge,
                    _ => SwitchConnectionState.Disconnected
                };
            }
        }

        #endregion

        #region Ctors

        public SwitchConnectionViewModel(SwitchConnector switchConnector) : base("Switch Connection")
        {
            _switchConnector = switchConnector;

            _switchConnector.StateChanged += switchConnector_StateChanged;
            _switchConnector.Connected += switchConnector_Connected;
            _switchConnector.Disconnected += switchConnector_Disconnected;
            _switchConnector.SwitchAddressChanged += switchConnector_SwitchAddressChanged;


            _connectCommand = new RelayCommand(ExcecuteConnectCommand, CanExecuteConnectCommand);
            _disconnectCommand = new RelayCommand(ExecuteDisconnectCommand, CanExecuteDisconnectCommand);
        }

        #endregion

        #region Command Methods

        private void ExcecuteConnectCommand()
        {
            if (!CanExecuteConnectCommand())
                return;

            SwitchBridgeClientConnectionSettings connectionSettings = new SwitchBridgeClientConnectionSettings(ClientAddress, ClientPort);

            try
            {
                _switchConnector.ConnectAsync(connectionSettings);
            }
            catch (Exception ex)
            {
                // TODO: Log error!
            }
        }

        private bool CanExecuteConnectCommand()
        {
            return _switchConnector.State == SwitchConnectorState.Disconnected
                && !string.IsNullOrWhiteSpace(ClientAddress)
                && ClientPort > 0;
        }

        private void ExecuteDisconnectCommand()
        {
            if (!CanExecuteDisconnectCommand())
                return;

            _switchConnector.Disconnect();
        }

        private bool CanExecuteDisconnectCommand() => _switchConnector.IsConnected;

        #endregion

        #region Methods

        private void switchConnector_Connected(object? sender, EventArgs e)
        {
            NotifyConnectionStatePropsChanged();

            _switchConnector.Update();
        }

        private void switchConnector_Disconnected(object? sender, EventArgs e)
        {
            NotifyConnectionStatePropsChanged();
        }

        private void switchConnector_SwitchAddressChanged(object? sender, EventArgs e)
        {
            NotifyConnectionStatePropsChanged();
        }

        private void switchConnector_StateChanged(object? sender, EventArgs e)
        {
            NotifyConnectionStatePropsChanged();
        }

        private void NotifyConnectionStatePropsChanged()
        {
            OnPropertyChanged(nameof(ConnectionState));
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(CanConnect));
            OnPropertyChanged(nameof(IsWaitingForSwitch));

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                _connectCommand.NotifyCanExecuteChanged();
                _disconnectCommand.NotifyCanExecuteChanged();
            });
        }


        #endregion
    }
}
