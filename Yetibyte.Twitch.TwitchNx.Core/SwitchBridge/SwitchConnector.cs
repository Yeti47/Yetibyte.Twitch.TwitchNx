using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchConnector : IDisposable
    {
        private bool _disposedValue;

        private readonly ISwitchBridgeClientFactory _clientFactory;
        private ISwitchBridgeClient? _client;
        private readonly System.Timers.Timer? _timer;

        private string _switchAddress = string.Empty;
        private readonly List<SwitchController> _controllers = new List<SwitchController>();

        public bool IsConnected => _client?.IsConnected ?? false;

        /// <summary>
        /// The interval in seconds at which the connector automatically requests
        /// a status update from the server. If the value is less than or equal to 0,
        /// no auto-updates will be performed.
        /// </summary>
        public float AutoUpdateFrequency { get; }

        public string SwitchAddress
        {
            get => _switchAddress;
            private set
            {
                if (_switchAddress == value)
                    return;

                _switchAddress = value;
                OnSwitchAddressChanged();
            }
        }

        public bool IsConnectedToSwitch => !string.IsNullOrWhiteSpace(SwitchAddress);

        public SwitchBridgeClientConnectionSettings ConnectionSettings => _client?.ConnectionSettings ?? SwitchBridgeClientConnectionSettings.CreateEmpty();

        public event EventHandler? Connected;
        public event EventHandler? Disconnected;
        public event EventHandler? SwitchAddressChanged;
        public event EventHandler<SwitchControllerAddedEventArgs>? ControllerAdded;
        public event EventHandler<SwitchControllerRemovedEventArgs>? ControllerRemoved;
        public event EventHandler? StatusUpdated;

        public SwitchConnector(ISwitchBridgeClientFactory clientFactory, float autoUpdateFrequency = 10)
        {
            _clientFactory = clientFactory;
            AutoUpdateFrequency = autoUpdateFrequency;

            if (AutoUpdateFrequency > 0)
            {
                _timer = new System.Timers.Timer(AutoUpdateFrequency * 1000);
                _timer.AutoReset = true;
                _timer.Elapsed += timer_Elapsed;
            }
        }

        private void timer_Elapsed(object? sender, ElapsedEventArgs e) => Update();

        public void Update()
        {
            if (_disposedValue || _client is null || !IsConnected)
                return;

            _client.GetSwitchAddresses();
            _client.GetSwitchStatus();
        }

        public bool Connect(SwitchBridgeClientConnectionSettings connectionSettings)
        {
            if (_client is not null)
                _client.ConnectionSettings = connectionSettings;
            else
            {
                _client = _clientFactory.CreateClient(connectionSettings);
                _client.MessageReceived += client_MessageReceived;
            }

            return _client.Connect();
        }

        private void client_MessageReceived(object? sender, SwitchBridgeMessageReceivedEventArgs e)
        {
            switch (e.Message.MessageType)
            {
                case StatusSwitchBridgeMessage.MESSAGE_TYPE:
                    StatusSwitchBridgeMessage statusMessage = (StatusSwitchBridgeMessage)e.Message;
                    ProcessStatusMessage(statusMessage);
                    break;
                case SwitchAddressesSwitchBridgeMessage.MESSAGE_TYPE:
                    SwitchAddressesSwitchBridgeMessage addressMessage = (SwitchAddressesSwitchBridgeMessage)e.Message;
                    ProcessAddressMessage(addressMessage);
                    break;
                default:
                    break;
            }
        }

        private void ProcessAddressMessage(SwitchAddressesSwitchBridgeMessage message)
        {
            SwitchAddress = message.Payload?.SwitchAddresses?.FirstOrDefault() ?? string.Empty;
        }

        private void ProcessStatusMessage(StatusSwitchBridgeMessage message)
        {
            foreach(DataTransfer.ControllerState controllerStateData in message.Payload.ControllerStates)
            {
                Models.ControllerState controllerState = Enum.TryParse<Models.ControllerState>(controllerStateData.State, true, out var controllerStatetemp)
                    ? controllerStatetemp
                    : Models.ControllerState.Initializing;

                if (!_controllers.Any(c => c.Id == controllerStateData.Id))
                {
                    Models.ControllerType controllerType = ControllerType.ProController;

                    if ("JOYCON_L".Equals(controllerStateData.Type, StringComparison.OrdinalIgnoreCase))
                        controllerType = ControllerType.JoyConLeft;
                    else if ("JOYCON_R".Equals(controllerStateData.Type, StringComparison.OrdinalIgnoreCase))
                        controllerType = ControllerType.JoyConRight;

                    SwitchController newController = new SwitchController(controllerStateData.Id, controllerType);

                    _controllers.Add(newController);
                    OnControllerAdded(newController);
                }

                SwitchController controller = _controllers.First(c => c.Id == controllerStateData.Id);

                controller.ErrorMessage = controllerStateData.Errors;
                controller.State = controllerState;

                OnStatusUpdated();
            }
        }

        public bool Disconnect()
        {
            return _client?.Disconnect() ?? false;
        }

        protected virtual void OnConnected()
        {
            var handler = Connected;
            handler?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnDisconnected()
        {
            var handler = Disconnected;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSwitchAddressChanged()
        {
            var handler = SwitchAddressChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnControllerAdded(SwitchController controller)
        {
            var handler = ControllerAdded;
            handler?.Invoke(this, new SwitchControllerAddedEventArgs(controller));
        }

        protected virtual void OnControllerRemoved(int controllerId)
        {
            var handler = ControllerRemoved;
            handler?.Invoke(this, new SwitchControllerRemovedEventArgs(controllerId));
        }

        protected virtual void OnStatusUpdated()
        {
            var handler = StatusUpdated;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_client is not null)
                    {
                        _client.MessageReceived -= client_MessageReceived;
                        _client.Dispose();
                    }

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                _client = null;
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SwitchConnector()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
