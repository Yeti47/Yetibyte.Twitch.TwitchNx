using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.Utilities
{
    public static class EnumerableExt
    {
        public static void MutableForeach<T>(this IList<T> list, Action<T> action)
        {
            List<T> copiedList = new List<T>(list);

            foreach (T item in copiedList)
                action(item);

            copiedList.Clear();
        }
    }
}
