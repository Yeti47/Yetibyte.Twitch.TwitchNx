namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public interface IControllerInput
    {
        ControllerInputType InputType { get; }

        string Macro { get; }
    }
}
