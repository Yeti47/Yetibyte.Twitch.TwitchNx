using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    [Serializable]
    public class CooldownGroup
    {
        [JsonInclude]
        private readonly Dictionary<PermissionLevel, float> _times = new Dictionary<PermissionLevel, float>();

        public string Name { get; set; }
        
        public float SharedTime { get; set; }

        [JsonIgnore]
        public TimeSpan SharedDuration => TimeSpan.FromSeconds(SharedTime);

        [JsonIgnore]
        public IEnumerable<(PermissionLevel permissionLevel, float time)> Times => _times.Select(kvp => (kvp.Key, kvp.Value));

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

    }
}
