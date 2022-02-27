namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public interface IMacroInstruction : IEnumerable<IControllerInput>
    {
        float Seconds { get; set; }
    }
}