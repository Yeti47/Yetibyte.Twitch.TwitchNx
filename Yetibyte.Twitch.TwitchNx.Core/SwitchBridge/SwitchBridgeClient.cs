using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer;
using System.Text.Json;
using System.Text.Json.Serialization;
using Websocket.Client;
using System.Linq;
using System.Reactive.Linq;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeClient : ISwitchBridgeClient
    {
        private readonly WebsocketClient _webSocket;
        private bool _disposedValue;

        public bool IsConnected => _webSocket.IsRunning;

        public string Url => $"ws://{ConnectionSettings.Address}:{ConnectionSettings.Port}";

        public SwitchBridgeClientConnectionSettings ConnectionSettings { get; set; }


        public event EventHandler<SwitchBridgeMessageReceivedEventArgs>? MessageReceived;
        public event EventHandler? Connected;
        public event EventHandler? Disconnected;

        public SwitchBridgeClient(SwitchBridgeClientConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;

            _webSocket = new WebsocketClient(new Uri(Url));

            _webSocket.ReconnectionHappened.Subscribe(r => OnConnected());
            _webSocket.MessageReceived.Subscribe(webSocket_OnMessage);
            _webSocket.DisconnectionHappened.Subscribe(d => OnDisconnected());

            //_webSocket.OnError += webSocket_OnError;
            //_webSocket.OnMessage += webSocket_OnMessage;
            //_webSocket.OnOpen += (o, e) => OnConnected();
            //_webSocket.OnClose += (o, e) => OnDisconnected();
        }

        //private void webSocket_OnError(object? sender, WebSocketSharp.ErrorEventArgs e)
        //{
        //    string test = "";
        //}

        //private void webSocket_OnMessage(object? sender, MessageEventArgs e)
        private void webSocket_OnMessage(ResponseMessage e)
        {
            SwitchBridgeMessage? baseMessage;

            try
            {
                baseMessage = JsonSerializer.Deserialize<SwitchBridgeMessage>(e.Text);
            }
            catch (Exception ex)
            {
                // TODO: Instead of throwing an exception, raise an event!
                // throw new SwitchBridgeResponseException(e.Data, innerException: ex);
                return;
            }

            if (baseMessage == null)
                throw new SwitchBridgeResponseException(e.Text);

            SwitchBridgeMessage? message;

            try
            {
                message = baseMessage.MessageType.ToUpper() switch
                {
                    SwitchAddressesSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<SwitchAddressesSwitchBridgeMessage>(e.Text),
                    StatusSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<StatusSwitchBridgeMessage>(e.Text),
                    CreateControllerSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<CreateControllerSwitchBridgeMessage>(e.Text),
                    RemoveControllerSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<RemoveControllerSwitchBridgeMessage>(e.Text),
                    MacroSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<MacroSwitchBridgeMessage>(e.Text),
                    MacroCompleteSwitchBridgeMessage.MESSAGE_TYPE => JsonSerializer.Deserialize<MacroCompleteSwitchBridgeMessage>(e.Text),
                    _ => null
                };

            }
            catch (Exception ex)
            {
                // TODO: Instead of throwing an exception, raise an event!
                // throw new SwitchBridgeResponseException(e.Data, innerException: ex);
                return;
            }

            if (message is not null)
                OnMessageReceived(message);
            else
            {
                // TODO: Instead of throwing an exception, raise an event!
                // throw new SwitchBridgeResponseException(e.Data);
            }
        }

        public bool Connect()
        {
            _webSocket.IsReconnectionEnabled = true;

            try
            {
                var startTask = _webSocket.Start();
                startTask.Wait();
            }
            catch (Exception ex)
            {
                throw new SwitchBridgeConnectionException(innerException: ex);
            }

            return true;
        }

        public bool Disconnect()
        {
            //var stopTask = _webSocket.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Close requested.");
            //stopTask.Wait();
            _webSocket.IsReconnectionEnabled = false;

            Task.Run(async () => await _webSocket.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Close requested."));

            return true;

            //return Task.Run(() => _webSocket.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Close requested.")).GetAwaiter().GetResult();

            //return stopTask.Result;
            //return stopTask.GetAwaiter().GetResult(); 
        }

        public bool ConnectAsync()
        {
            //try
            //{
            //    _webSocket.ConnectAsync();
            //}
            //catch (Exception ex)
            //{
            //    throw new SwitchBridgeConnectionException(innerException: ex);
            //}

            //return true;

            //Task.Run(() => _webSocket.Connect()).ContinueWith(t =>
            //{
            //    if (t.IsFaulted)
            //    {
            //        // Raise OnError event
            //    }

            //});

            _webSocket.IsReconnectionEnabled = true;

            _webSocket.Start();

            return true;
        }

        public bool DisconnectAsync()
        {
            //_webSocket.CloseAsync();

            _webSocket.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Close requested.");

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

        public string ExecuteMacro(string macro, int controllerId)
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

            return message.Id;
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
