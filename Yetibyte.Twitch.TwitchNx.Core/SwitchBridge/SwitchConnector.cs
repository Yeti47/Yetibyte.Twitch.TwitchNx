using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.Models;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchConnector : IDisposable
    {
        #region Fields

        private readonly ISwitchBridgeClientFactory _clientFactory;
        private ISwitchBridgeClient? _client;
        private readonly List<SwitchController> _controllers = new List<SwitchController>();
        private readonly System.Timers.Timer? _timer;

        private bool _disposedValue;
        private string _switchAddress = string.Empty;
        private SwitchConnectorState _state = SwitchConnectorState.Disconnected;

        #endregion

        #region Props

        public IList<SwitchController> Controllers => new ReadOnlyCollection<SwitchController>(_controllers);

        public SwitchConnectorState State
        {
            get => _state;
            set
            {
                _state = value;
                OnStateChanged();
            }
        }

        public bool IsConnected => _state == SwitchConnectorState.Connected;

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

        #endregion

        #region Events

        public event EventHandler? Connected;
        public event EventHandler? Disconnected;
        public event EventHandler? SwitchAddressChanged;
        public event EventHandler? StatusUpdated;
        public event EventHandler? StateChanged;

        public event EventHandler<AddingSwitchControllerEventArgs>? AddingController;
        public event EventHandler<SwitchControllerAddedEventArgs>? ControllerAdded;
        public event EventHandler<SwitchControllerRemovedEventArgs>? ControllerRemoved;

        #endregion

        #region Ctors

        public SwitchConnector(ISwitchBridgeClientFactory clientFactory, float autoUpdateFrequency = 10)
        {
            _clientFactory = clientFactory;
            AutoUpdateFrequency = autoUpdateFrequency;

            if (AutoUpdateFrequency > 0)
            {
                _timer = new System.Timers.Timer(AutoUpdateFrequency * 1000)
                {
                    AutoReset = true
                };
                _timer.Elapsed += timer_Elapsed;
                _timer.Start();
            }
        }

        #endregion

        #region Methods

        private void timer_Elapsed(object? sender, ElapsedEventArgs e) => Update();

        public bool AddController(Models.ControllerType controllerType)
        {
            if (_client is null || _disposedValue || !IsConnected)
                return false;

            string controllerTypeName = SwitchController.CONTROLLER_TYPE_NAME_PRO_CONTROLLER;

            if (controllerType == Models.ControllerType.JoyConLeft)
                controllerTypeName = SwitchController.CONTROLLER_TYPE_NAME_JOYCON_LEFT;
            else if (controllerType == Models.ControllerType.JoyConRight)
                controllerTypeName = SwitchController.CONTROLLER_TYPE_NAME_JOYCON_RIGHT;

            OnAddingController(controllerType);

            _client.CreateController(controllerTypeName);

            return true;
        }

        public bool RemoveController(int controllerId)
        {
            if (_client is null || _disposedValue || !IsConnected)
                return false;

            _client.RemoveController(controllerId);

            return true;
        }

        /// <summary>
        /// Fetches the current state of the Switch from the server.
        /// </summary>
        public void Update()
        {
            if (_disposedValue || _client is null || !IsConnected || !_client.IsConnected)
                return;

            try
            {
                _client.GetSwitchAddresses();
                _client.GetSwitchStatus();
            }
            catch(Exception ex)
            {
                // TODO: proper error handling!
            }
        }

        public bool Connect(SwitchBridgeClientConnectionSettings connectionSettings)
        {
            InitializeClient(connectionSettings);

            State = SwitchConnectorState.Connecting;

            return _client?.Connect() ?? false;
        }

        private void InitializeClient(SwitchBridgeClientConnectionSettings connectionSettings)
        {
            if (_client is not null)
            {
                _client.MessageReceived -= client_MessageReceived;
                _client.Connected -= client_Connected;
                _client.Disconnected -= client_Disconnected;

                _client.Dispose();
            }

            _client = _clientFactory.CreateClient(connectionSettings);
            _client.MessageReceived += client_MessageReceived;
            _client.Connected += client_Connected;
            _client.Disconnected += client_Disconnected;
        }

        public bool DisconnectAsync() => _client?.DisconnectAsync() ?? false;

        public bool ConnectAsync(SwitchBridgeClientConnectionSettings connectionSettings)
        {
            InitializeClient(connectionSettings);

            State = SwitchConnectorState.Connecting;

            try
            {
                return _client?.ConnectAsync() ?? false;
            }
            catch (Exception)
            {
                State = SwitchConnectorState.Disconnected;
                throw;
            }
            
        }

        private void client_Connected(object? sender, EventArgs e)
        {
            OnConnected();
        }
        private void client_Disconnected(object? sender, EventArgs e)
        {
            OnDisconnected();
        }

        private void client_MessageReceived(object? sender, SwitchBridgeMessageReceivedEventArgs e)
        {
            switch (e.Message.MessageType.ToUpper())
            {
                case StatusSwitchBridgeMessage.MESSAGE_TYPE:
                    StatusSwitchBridgeMessage statusMessage = (StatusSwitchBridgeMessage)e.Message;
                    ProcessStatusMessage(statusMessage);
                    break;
                case SwitchAddressesSwitchBridgeMessage.MESSAGE_TYPE:
                    SwitchAddressesSwitchBridgeMessage addressMessage = (SwitchAddressesSwitchBridgeMessage)e.Message;
                    ProcessAddressMessage(addressMessage);
                    break;
                case CreateControllerSwitchBridgeMessage.MESSAGE_TYPE:
                    CreateControllerSwitchBridgeMessage createControllerMesage = (CreateControllerSwitchBridgeMessage)e.Message;
                    ProcessCreateControllerMessage(createControllerMesage);
                    break;
                case RemoveControllerSwitchBridgeMessage.MESSAGE_TYPE:
                    RemoveControllerSwitchBridgeMessage removeControllerMessage = (RemoveControllerSwitchBridgeMessage)e.Message;
                    ProcessRemoveControllerMessage(removeControllerMessage);
                    break;
                default:
                    // TODO: Proper handling of unknown message types.
                    break;
            }
        }

        private void ProcessCreateControllerMessage(CreateControllerSwitchBridgeMessage message)
        {
            if (message.IsError)
            {
                // TODO: proper error handling!
                return;
            }

            Update();
        }

        private void ProcessRemoveControllerMessage(RemoveControllerSwitchBridgeMessage message)
        {
            if (message.IsError)
            {
                // TODO: proper error handling!
                return;
            }

            Update();
        }

        private void ProcessAddressMessage(SwitchAddressesSwitchBridgeMessage message)
        {
            if (message.IsError)
            {
                // TODO: proper error handling!
                return;
            }

            SwitchAddress = message.Payload?.SwitchAddresses?.FirstOrDefault() ?? string.Empty;
        }

        private void ProcessStatusMessage(StatusSwitchBridgeMessage message)
        {
            if (message.IsError)
            {
                // TODO: proper error handling!
                return;
            }

            foreach (DataTransfer.ControllerState controllerStateData in message.Payload.ControllerStates)
            {
                Models.ControllerState controllerState = Enum.TryParse<Models.ControllerState>(controllerStateData.State, true, out var controllerStatetemp)
                    ? controllerStatetemp
                    : Models.ControllerState.NotSynced;

                if (!_controllers.Any(c => c.Id == controllerStateData.Id))
                {
                    Models.ControllerType controllerType = ControllerType.ProController;

                    if (SwitchController.CONTROLLER_TYPE_NAME_JOYCON_LEFT.Equals(controllerStateData.Type, StringComparison.OrdinalIgnoreCase))
                        controllerType = ControllerType.JoyConLeft;
                    else if (SwitchController.CONTROLLER_TYPE_NAME_JOYCON_RIGHT.Equals(controllerStateData.Type, StringComparison.OrdinalIgnoreCase))
                        controllerType = ControllerType.JoyConRight;

                    SwitchController newController = new SwitchController(controllerStateData.Id, controllerType);

                    _controllers.Add(newController);
                    OnControllerAdded(newController);
                }

                SwitchController controller = _controllers.First(c => c.Id == controllerStateData.Id);

                controller.ErrorMessage = controllerStateData.Errors;
                controller.State = controllerState;
            }

            List<SwitchController> controllersToRemove = new List<SwitchController>();

            foreach(SwitchController controller in _controllers)
            {
                if (!message.Payload.ControllerStates.Any(c => c.Id == controller.Id))
                    controllersToRemove.Add(controller);
            }

            foreach(SwitchController controller in controllersToRemove)
            {
                _controllers.Remove(controller);
                OnControllerRemoved(controller.Id);
            }

            OnStatusUpdated();
        }

        public bool Disconnect() => _client?.Disconnect() ?? false;

        protected virtual void OnConnected()
        {
            State = SwitchConnectorState.Connected;

            var handler = Connected;
            handler?.Invoke(this, EventArgs.Empty);

        }
        protected virtual void OnDisconnected()
        {
            SwitchAddress = String.Empty;
            State = SwitchConnectorState.Disconnected;

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

        protected virtual void OnAddingController(ControllerType controllerType)
        {
            var handler = AddingController;
            handler?.Invoke(this, new AddingSwitchControllerEventArgs(controllerType));
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

        protected virtual void OnStateChanged()
        {
            var handler = StateChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        #region IDisposable Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_client is not null)
                    {
                        _client.MessageReceived -= client_MessageReceived;
                        _client.Connected -= client_Connected;
                        _client.Disconnected -= client_Disconnected;

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

        #endregion

        #endregion

    }
}
