using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;


namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{
    public class TwitchIrcClient : IIrcClient
    {
        private readonly TwitchClient _client;

        public string UserName { get; private set; } = string.Empty;

        public string ChannelName { get; private set; } = string.Empty;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string AuthToken { get; private set; } = string.Empty;

        public bool IsConnected => _client.IsConnected;

        public bool IsInitialized => !string.IsNullOrWhiteSpace(UserName)
                                  && !string.IsNullOrWhiteSpace(ChannelName)
                                  && !string.IsNullOrWhiteSpace(AuthToken);   

        public event EventHandler<IrcConnectedEventArgs>? Connected;
        public event EventHandler<IrcDisconnectedEventArgs>? Disconnected;
        public event EventHandler<IrcMessageEventArgs>? MessageReceived;
        public event EventHandler<IrcMessageEventArgs>? MessageSent;
        public event EventHandler<IrcErrorEventArgs>? ErrorOccurred;
        public event EventHandler<IrcErrorEventArgs>? ConnectionErrorOccurred;

        public TwitchIrcClient()
        {
            _client = new TwitchClient();
            _client.OnMessageSent += client_OnMessageSent;
            _client.OnMessageReceived += client_OnMessageReceived;
            _client.OnConnected += client_OnConnected;
            _client.OnDisconnected += client_OnDisconnected;
            _client.OnError += client_OnError;
            _client.OnConnectionError += client_OnConnectionError;
        }

        private void client_OnConnectionError(object? sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
        {
            OnConnectionErrorOccurred(new IrcErrorEventArgs(e.Error.Message));
        }

        private void client_OnError(object? sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
        {
            OnErrorOccurred(new IrcErrorEventArgs(e.Exception.Message));
        }

        private void client_OnDisconnected(object? sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            OnDisconnected(new IrcDisconnectedEventArgs());
        }

        private void client_OnConnected(object? sender, TwitchLib.Client.Events.OnConnectedArgs e)
        { 
            OnConnected(new IrcConnectedEventArgs(e.AutoJoinChannel));
        }

        private void client_OnMessageReceived(object? sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            IrcMember author = new IrcMember(e.ChatMessage.Username, e.ChatMessage.DisplayName)
            {
                Color = System.Drawing.ColorTranslator.FromHtml(e.ChatMessage.ColorHex),
                IsMod = e.ChatMessage.IsModerator,
                IsSubscriber = e.ChatMessage.IsSubscriber,
                IsMe = e.ChatMessage.IsMe,
                IsOwner = e.ChatMessage.IsBroadcaster
            };

            IrcMessage ircMessage = new IrcMessage(e.ChatMessage.Id, author, e.ChatMessage.Channel, e.ChatMessage.Message, DateTime.Now);

            OnMessageReceived(ircMessage);
        }

        private void client_OnMessageSent(object? sender, TwitchLib.Client.Events.OnMessageSentArgs e)
        {
            string id = Guid.NewGuid().ToString();

            IrcMember member = new IrcMember(UserName)
            {
                IsMe = true,
                IsMod = e.SentMessage.IsModerator,
                IsSubscriber = e.SentMessage.IsSubscriber,
                IsOwner = UserName.Equals(ChannelName, StringComparison.OrdinalIgnoreCase)
            };

            IrcMessage ircMessage = new IrcMessage(id, member, ChannelName, e.SentMessage.Message, DateTime.Now);

            OnMessageSent(ircMessage);
        }

        public void Initialize(string userName, string channelName, string authToken)
        {
            UserName = userName;
            ChannelName = channelName;
            AuthToken = authToken;

            _client.Initialize(new ConnectionCredentials(UserName, AuthToken), ChannelName, autoReListenOnExceptions: false);
        }

        public bool Connect()
        {
            if (IsConnected)
                return false;

            bool success = _client.Connect();

            if (success)
                _client.JoinChannel(ChannelName);

            return success;
        }

        public bool Disconnect()
        {
            if (!IsConnected)
                return false;

            _client.Disconnect();

            return true;
        }

        public bool SendMessage(string message)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Cannot send message. Twitch client not connected.");

            _client.SendMessage(ChannelName, message);

            return true;
        }

        protected virtual void OnMessageSent(IrcMessage ircMessage)
        {
            var handler = MessageSent;
            handler?.Invoke(this, new IrcMessageEventArgs(ircMessage));
        }

        protected virtual void OnMessageReceived(IrcMessage ircMessage)
        {
            var handler = MessageReceived;
            handler?.Invoke(this, new IrcMessageEventArgs(ircMessage));
        }

        protected virtual void OnConnected(IrcConnectedEventArgs ircConnectedEventArgs)
        {
            var handler = Connected;
            handler?.Invoke(this, ircConnectedEventArgs);
        }

        protected virtual void OnDisconnected(IrcDisconnectedEventArgs ircDisconnectedEventArgs)
        {
            var handler = Disconnected;
            handler?.Invoke(this, ircDisconnectedEventArgs);
        }

        protected virtual void OnErrorOccurred(IrcErrorEventArgs ircErrorEventArgs)
        {
            var handler = ErrorOccurred;
            handler?.Invoke(this, ircErrorEventArgs);   
        }

        protected virtual void OnConnectionErrorOccurred(IrcErrorEventArgs ircErrorEventArgs)
        {
            var handler = ConnectionErrorOccurred;
            handler?.Invoke(this, ircErrorEventArgs);
        }

    }
}
