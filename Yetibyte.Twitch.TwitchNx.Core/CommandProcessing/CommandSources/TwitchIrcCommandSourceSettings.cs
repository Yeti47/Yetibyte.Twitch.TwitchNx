namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    [Serializable]
    public class TwitchIrcCommandSourceSettings : ICommandSourceSettings
    {
        public const string COMMAND_SOURCE_ID = "TwitchIrcCommandSource";

        [Newtonsoft.Json.JsonIgnore]
        public Type CommandSourceType => typeof(IrcCommandSource);

        [Newtonsoft.Json.JsonIgnore]
        public string CommandSourceId => COMMAND_SOURCE_ID;

        
        public string ChannelName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonConverter(typeof(TwitchIrcAuthTokenJsonConverter))]
        public string AuthToken { get; set; } = string.Empty;

    }
}
