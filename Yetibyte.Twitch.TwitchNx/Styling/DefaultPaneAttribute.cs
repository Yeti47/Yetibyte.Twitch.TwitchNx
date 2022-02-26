using System;

namespace Yetibyte.Twitch.TwitchNx.Styling
{
    internal class DefaultPaneAttribute : Attribute
    {
        public string DefaultPaneName { get; private set; }

        public DefaultPaneAttribute(string paneName)
        {
            DefaultPaneName = paneName;
        }

    }
}
