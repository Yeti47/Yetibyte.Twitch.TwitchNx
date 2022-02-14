using WebSocketSharp;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeClient : ISwitchBridgeClient
    {
        private readonly WebSocket _webSocket;
        private bool _disposedValue;

        public bool IsConnected => _webSocket.IsAlive;

        public string Url => $"ws://{ConnectionSettings.Address}:{ConnectionSettings.Port}";

        public SwitchBridgeClientConnectionSettings ConnectionSettings { get; set; }


        public event EventHandler<SwitchBridgeMessageReceivedEventArgs>? MessageReceived;
        public event EventHandler? Connected;
        public event EventHandler? Disconnected;

        public SwitchBridgeClient(SwitchBridgeClientConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;

            _webSocket = new WebSocket(Url);
            _webSocket.OnMessage += webSocket_OnMessage;
            _webSocket.OnOpen += (o, e) => OnConnected();
            _webSocket.OnClose += (o, e) => OnDisconnected();
        }

        private void webSocket_OnMessage(object? sender, MessageEventArgs e)
        {
            SwitchBridgeMessage? baseMessage;

            try
            {
                baseMessage = JsonSerializer.Deserialize<SwitchBridgeMessage>(e.Data);
            }
            catch (Exception ex)
            {
                throw new SwitchBridgeResponseException(e.Data, innerException: ex);
            }

            if (baseMessage == null)
                throw new SwitchBridgeResponseException(e.Data);

            SwitchBridgeMessage? message;

            try
            {
                message = baseMessage.MessageType.ToUpper() switch
                {
                    SwitchAddressesSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<SwitchAddressesSwitchBridgeMessage>(e.Data),
                    StatusSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<StatusSwitchBridgeMessage>(e.Data),
                    CreateControllerSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<CreateControllerSwitchBridgeMessage>(e.Data),
                    RemoveControllerSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<RemoveControllerSwitchBridgeMessage>(e.Data),
                    MacroSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<MacroSwitchBridgeMessage>(e.Data),
                    MacroCompleteSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<MacroCompleteSwitchBridgeMessage>(e.Data),
                    _ => null
                };

            }
            catch (Exception ex)
            {
                throw new SwitchBridgeResponseException(e.Data, innerException: ex);
            }

            if (message is not null)
                OnMessageReceived(message);
            else
                throw new SwitchBridgeResponseException(e.Data);
        }

        public bool Connect()
        {
            try
            {
                _webSocket.Connect();
            }
            catch (Exception ex)
            {
                throw new SwitchBridgeConnectionException(innerException: ex);
            }

            return true;
        }

        public void CreateController(string controllerType)
        {
            if (!IsConnected)
                throw new InvalidOperationException($"{nameof(SwitchBridgeClient)} not connected.");
                
            string messageId = GenerateMessageId();

            CreateControllerSwitchBridgeMessage message = new CreateControllerSwitchBridgeMessage(
                messageId,
                new ControllerPayload(ControllerType: controllerType)
            );

            string messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                _webSocket.Send(messageJson);
            }
            catch(Exception ex)
            {
                throw new SwitchBridgeOperationException(nameof(CreateController), innerException: ex);
            }
            
        }

        public bool Disconnect()
        {
            _webSocket.Close();

            return true;
        }

        public void ExecuteMacro(string macro, int controllerId)
        {
            if (!IsConnected)
                throw new InvalidOperationException($"{nameof(SwitchBridgeClient)} not connected.");

            string messageId = GenerateMessageId();

            MacroSwitchBridgeMessage message = new MacroSwitchBridgeMessage(
                messageId, 
                new MacroPayload(macro, controllerId)
            );

            string messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                _webSocket.Send(messageJson);
            }
            catch (Exception ex)
            {
                throw new SwitchBridgeOperationException(nameof(ExecuteMacro), innerException: ex);
            }
        }

        public void GetSwitchAddresses()
        {
            if (!IsConnected)
                throw new InvalidOperationException($"{nameof(SwitchBridgeClient)} not connected.");

            string messageId = GenerateMessageId();

            SwitchAddressesSwitchBridgeMessage message = new SwitchAddressesSwitchBridgeMessage(
                messageId
            );

            string messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                _webSocket.Send(messageJson);
            }
            catch (Exception ex)
            {
                throw new SwitchBridgeOperationException(nameof(GetSwitchAddresses), innerException: ex);
            }
        }

        public void GetSwitchStatus()
        {
            if (!IsConnected)
                throw new InvalidOperationException($"{nameof(SwitchBridgeClient)} not connected.");

            string messageId = GenerateMessageId();

            StatusSwitchBridgeMessage message = new StatusSwitchBridgeMessage(
                messageId
            );

            string messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                _webSocket.Send(messageJson);
            }
            catch (Exception ex)
            {
                throw new SwitchBridgeOperationException(nameof(GetSwitchStatus), innerException: ex);
            }
        }

        public void RemoveController(int controllerId)
        {
            if (!IsConnected)
                throw new InvalidOperationException($"{nameof(SwitchBridgeClient)} not connected.");

            string messageId = GenerateMessageId();

            RemoveControllerSwitchBridgeMessage message = new RemoveControllerSwitchBridgeMessage(
                messageId,
                new ControllerPayload(ControllerId: controllerId)
            );

            string messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                _webSocket.Send(messageJson);
            }
            catch (Exception ex)
            {
                throw new SwitchBridgeOperationException(nameof(RemoveController), innerException: ex);
            }
        }

        protected virtual void OnMessageReceived(SwitchBridgeMessage message)
        {
            var handler = MessageReceived;
            handler?.Invoke(this, new SwitchBridgeMessageReceivedEventArgs(message));
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

        private string GenerateMessageId() => Guid.NewGuid().ToString();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    ((IDisposable)_webSocket).Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SwitchBridgeClient()
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
