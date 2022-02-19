using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class CooldownGroup
    {
        private readonly Dictionary<PermissionLevel, float> _times = new Dictionary<PermissionLevel, float>();

        public string Name { get; set; }
        
        public float SharedTime { get; set; }

        public TimeSpan SharedDuration => TimeSpan.FromSeconds(SharedTime);

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
