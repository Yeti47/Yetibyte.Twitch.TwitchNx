using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class Macro
    {
        [Newtonsoft.Json.JsonIgnore]
        public double TotalSeconds => throw new NotImplementedException();
    }
}
