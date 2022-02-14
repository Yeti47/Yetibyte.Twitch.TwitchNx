namespace Yetibyte.Twitch.TwitchNx.Core.SwitchBridge
{
    public class SwitchBridgeOperationException : SwitchBridgeException
    {
        public const string DEFAULT_MESSAGE = "An error occurred while trying to perform a Switch Bridge operation.";

        public string OperationName { get; }

        public SwitchBridgeOperationException(string operationName, string message = DEFAULT_MESSAGE, Exception? innerException = null) : base(message, innerException)
        {
            OperationName = operationName;
        }
    }
}