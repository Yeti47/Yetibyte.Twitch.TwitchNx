using Yetibyte.Twitch.TwitchNx.Core.Common;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    /// <summary>
    /// Abstraction of a command source that can produce commands.
    /// <para>Currently planned sources:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <term>IRC</term>
    ///     <description>Commands are triggered by parsing IRC messages.</description>
    ///   </item>
    ///   <item>
    ///     <term>Twitch Extension</term>
    ///     <description>Commands are triggered via a Twitch extension.</description>
    ///   </item>
    /// </list>
    /// </summary>
    public interface ICommandSource : IRunnable
    {

        event EventHandler<CommandReceivedEventArgs>? CommandReceived;

        void NotifyCommandProcessed(CommandProcessingResult commandProcessingResult);

    }
}
