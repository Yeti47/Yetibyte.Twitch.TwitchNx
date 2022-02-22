namespace Yetibyte.Twitch.TwitchNx.Core.Irc
{
    public class IrcMessage
    {
        public string Id { get; }
        
        public IrcMember Author { get; }
        public string Channel { get; }

        public string Content { get; }

        public DateTime Timestamp { get; }

        public IrcMessage(string id, IrcMember author, string channel, string content, DateTime timestamp)
        {
            Id = id;
            Author = author;
            Channel = channel;
            Content = content;
            Timestamp = timestamp;
        }

        public override string ToString() => $"{Author.DisplayName}: {Content}";

    }
}
