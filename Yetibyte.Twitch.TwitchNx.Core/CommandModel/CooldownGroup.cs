using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Core.Common;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    [Serializable]
    public class CooldownGroup
    {
        [JsonInclude]
        private readonly Dictionary<PermissionLevel, float> _times = new Dictionary<PermissionLevel, float>();
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                string oldName = _name;
                _name = value;
                OnNameChanged(oldName);
            }
        }

        public float SharedTime { get; set; }

        public string UserCooldownMessage { get; set; } = "{USER}, please wait another {TIME} seconds before using this command again.";
        public string SharedCooldownMessage { get; set; } = "{USER}, this command is not ready yet. Please wait another {TIME} seconds.";

        [JsonIgnore]
        public TimeSpan SharedDuration => TimeSpan.FromSeconds(SharedTime);

        [JsonIgnore]
        public IEnumerable<(PermissionLevel permissionLevel, float time)> Times => _times.Select(kvp => (kvp.Key, kvp.Value));

        public event EventHandler<NameChangedEventArgs>? NameChanged;

        public CooldownGroup(string name)
        {
            Name = name;
        }

        public void SetTime(PermissionLevel permissionLevel, float time)
        {
            _times[permissionLevel] = time;
        }

        public float GetTime(PermissionLevel permissionLevel)
        {
            int permissionLevelValue = (int)permissionLevel;

            while (permissionLevelValue >= 0)
            {
                if (_times.ContainsKey((PermissionLevel)permissionLevelValue))
                    return _times[(PermissionLevel)permissionLevelValue];

                permissionLevelValue--;
            }

            return 0;
        }

        public string GetUserCooldownMessage(string userName, double remainingSeconds)
        {
            return GetCooldownMessage(UserCooldownMessage, userName, remainingSeconds);
        }

        public string GetSharedCooldownMessage(string userName, double remainingSeconds)
        {
            return GetCooldownMessage(SharedCooldownMessage, userName, remainingSeconds);
        }

        private string GetCooldownMessage(string messageTemplate, string userName, double remainingSeconds)
        {
            return messageTemplate
                .Replace("{USER}", userName)
                .Replace("{TIME}", remainingSeconds.ToString(".1"));
        }

        protected virtual void OnNameChanged(string oldName)
        {
            var handler = NameChanged;
            handler?.Invoke(this, new NameChangedEventArgs(oldName, Name));
        }

    }
}
