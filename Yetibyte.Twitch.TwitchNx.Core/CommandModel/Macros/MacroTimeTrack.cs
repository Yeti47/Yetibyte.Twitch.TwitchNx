﻿using System.Collections;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros
{
    [Serializable]
    public class MacroTimeTrack : IList<MacroTimeTrackElement>
    {
        [Newtonsoft.Json.JsonProperty("Elements")]
        private List<MacroTimeTrackElement> _elements = new List<MacroTimeTrackElement>();


        #region IList implementation

        [Newtonsoft.Json.JsonIgnore]
        public MacroTimeTrackElement this[int index] { 
            get => ((IList<MacroTimeTrackElement>)_elements)[index]; 
            set => ((IList<MacroTimeTrackElement>)_elements)[index] = value; 
        }

        [Newtonsoft.Json.JsonIgnore]
        public int Count => ((ICollection<MacroTimeTrackElement>)_elements).Count;

        [Newtonsoft.Json.JsonIgnore]
        public bool IsReadOnly => false;

        public void Add(MacroTimeTrackElement item)
        {
            ((ICollection<MacroTimeTrackElement>)_elements).Add(item);
        }

        public void Clear()
        {
            ((ICollection<MacroTimeTrackElement>)_elements).Clear();
        }

        public bool Contains(MacroTimeTrackElement item)
        {
            return ((ICollection<MacroTimeTrackElement>)_elements).Contains(item);
        }

        public void CopyTo(MacroTimeTrackElement[] array, int arrayIndex)
        {
            ((ICollection<MacroTimeTrackElement>)_elements).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MacroTimeTrackElement> GetEnumerator()
        {
            return ((IEnumerable<MacroTimeTrackElement>)_elements).GetEnumerator();
        }

        public int IndexOf(MacroTimeTrackElement item)
        {
            return ((IList<MacroTimeTrackElement>)_elements).IndexOf(item);
        }

        public void Insert(int index, MacroTimeTrackElement item)
        {
            ((IList<MacroTimeTrackElement>)_elements).Insert(index, item);
        }

        public bool Remove(MacroTimeTrackElement item)
        {
            return ((ICollection<MacroTimeTrackElement>)_elements).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<MacroTimeTrackElement>)_elements).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_elements).GetEnumerator();
        }

        #endregion

        public TimeSpan GetTotalDuration() => !_elements.Any() ? TimeSpan.Zero : _elements.Max(e => e.EndTime);
    }
}
