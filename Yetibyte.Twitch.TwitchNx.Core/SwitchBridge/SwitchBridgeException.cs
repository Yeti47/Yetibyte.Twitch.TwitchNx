namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public abstract class SwitchBridgeException : Exception
    {
        public SwitchBridgeException(string message, Exception? innerException = null) : base(message, innerException)
        {

        }
    }
}