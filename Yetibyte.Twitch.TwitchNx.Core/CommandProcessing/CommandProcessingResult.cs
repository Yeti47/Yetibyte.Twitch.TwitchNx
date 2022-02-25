namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing
{
    public record CommandProcessingResult(
        Command Command,
        bool Success, 
        bool IsMatch, 
        bool WasEnqueued, 
        TimeSpan SharedTimeRemaining = default(TimeSpan),
        TimeSpan TimeRemaining = default(TimeSpan)
    );
}
