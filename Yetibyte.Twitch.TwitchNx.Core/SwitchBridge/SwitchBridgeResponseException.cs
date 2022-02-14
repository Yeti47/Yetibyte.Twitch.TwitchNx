namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeResponseException : SwitchBridgeException
    {
        public const string DEFAULT_MESSAGE = "Could not process incoming message from Switch Bridge server.";

        public string MessageData { get; } 

        public SwitchBridgeResponseException(string data, string message = DEFAULT_MESSAGE, Exception? innerException = null) : base(message, innerException)
        {
            MessageData = data;
        }

    }
}