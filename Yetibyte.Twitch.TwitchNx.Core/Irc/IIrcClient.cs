using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{

    public interface IIrcClient
    {
        event EventHandler<IrcConnectedEventArgs>? Connected;
        event EventHandler<IrcDisconnectedEventArgs>? Disconnected;

        event EventHandler<IrcMessageEventArgs>? MessageReceived;
        event EventHandler<IrcMessageEventArgs>? MessageSent;

        event EventHandler<IrcErrorEventArgs>? ErrorOccurred;
        event EventHandler<IrcErrorEventArgs>? ConnectionErrorOccurred;

        string UserName { get; }
        string ChannelName { get; }
        string AuthToken { get; }

        bool IsConnected { get; }
        bool IsInitialized { get; }

        void Initialize(string userName, string channelName, string authToken);

        bool Connect();
        bool Disconnect();

        bool SendMessage(string message);

    }
}
