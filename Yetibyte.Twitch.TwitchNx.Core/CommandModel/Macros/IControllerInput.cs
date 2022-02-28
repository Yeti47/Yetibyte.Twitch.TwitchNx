namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    public interface IControllerInput
    {
        ControllerInputType InputType { get; }

        string GetMacro();
    }
}
