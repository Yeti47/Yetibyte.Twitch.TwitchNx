using System;
using System.Threading;
using Yetibyte.Twitch.TwitchNx.Core.SwitchBridge.DataTransfer;

namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class MockSwitchBridgeClient : ISwitchBridgeClient
    {
        public float ConnectionTimeSeconds { get; set; } = 1f;
        public float DisconnectionTimeSeconds { get; set; } = 1f;

        public bool IsConnected { get; private set; }

        public event EventHandler<SwitchBridgeMessageReceivedEventArgs>? MessageReceived;
        public event EventHandler? Connected;
        public event EventHandler? Disconnected;

        public MockSwitchBridgeClient()
        {
        }

        public bool Connect()
        {
            if (IsConnected)
                return false;
            
            Thread.Sleep(Convert.ToInt32(ConnectionTimeSeconds * 1000));

            IsConnected = true;

            OnConnected();

            return IsConnected;
        }

        public bool ConnectAsync()
        {
            if (IsConnected)
                return false;

            Task.Delay(Convert.ToInt32(ConnectionTimeSeconds * 1000)).ContinueWith(t =>
            {
                IsConnected = true;
                OnConnected();
            });

            return true;
        }

        public void CreateController(string controllerType)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            if (!IsConnected)
                return false;

            Thread.Sleep(Convert.ToInt32(DisconnectionTimeSeconds * 1000));

            IsConnected = false;

            OnDisconnected();

            return true;
        }

        public bool DisconnectAsync()
        {
            if (!IsConnected)
                return false;

            Task.Delay(Convert.ToInt32(ConnectionTimeSeconds * 1000)).ContinueWith(t =>
            {
                IsConnected = false;
                OnDisconnected();
            });

            return true;
        }

        public void Dispose()
        {
            // nothing to dispose
        }

        public string ExecuteMacro(string macro, int controllerId)
        {
            float macroDuration = CommandModel.Macros.Macro.GetMacroDuration(macro);

            string mockMacroMessageId = Guid.NewGuid().ToString();

            MacroSwitchBridgeMessage macroMessage = new MacroSwitchBridgeMessage(mockMacroMessageId, new MacroPayload(macro, controllerId));

            OnMessageReceived(macroMessage);

            Task.Delay(Convert.ToInt32(macroDuration * 1000)).ContinueWith(t =>
            {
                string macroCompleteMsgId = Guid.NewGuid().ToString();
                
                MacroCompleteSwitchBridgeMessage macroCompleteMessage = new MacroCompleteSwitchBridgeMessage(
                    macroCompleteMsgId,
                    new MacroCompletePayload(macroMessage.Id, controllerId, macroMessage.Id)
                );

                OnMessageReceived(macroCompleteMessage);
            });

            return mockMacroMessageId;
        }

        public void GetSwitchAddresses()
        {
            throw new NotImplementedException();
        }

        public void GetSwitchStatus()
        {
            throw new NotImplementedException();
        }

        public void RemoveController(int controllerId)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnMessageReceived(SwitchBridgeMessage message)
        {
            SwitchBridgeMessageReceivedEventArgs eventArgs = new SwitchBridgeMessageReceivedEventArgs(message);

            var handler = MessageReceived;
            handler?.Invoke(this, eventArgs);
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
    }
}
