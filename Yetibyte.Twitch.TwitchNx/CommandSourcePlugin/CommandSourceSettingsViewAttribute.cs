using System;

namespace Yetibyte.Twitch.TwitchNx.CommandSourcePlugin
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandSourceSettingsViewAttribute : Attribute
    {
        public Type ViewModelType { get; }

        public CommandSourceSettingsViewAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }
}
