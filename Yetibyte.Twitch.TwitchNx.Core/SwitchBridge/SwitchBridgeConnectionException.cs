namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeConnectionException: Exception
    {
        public const string DEFAULT_MESSAGE = "Could not establish connection to server. See inner exception for details.";

        public SwitchBridgeConnectionException(string message = DEFAULT_MESSAGE, Exception? innerException = null): base(message, innerException)
        {

        }
    }
}
