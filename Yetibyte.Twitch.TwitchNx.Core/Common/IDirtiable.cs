using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.Common
{
    public interface IDirtiable
    {
        bool IsDirty { get; }

        event EventHandler? IsDirtyChanged;

        void MarkDirty();
        void UnmarkDirty();
    }
}
