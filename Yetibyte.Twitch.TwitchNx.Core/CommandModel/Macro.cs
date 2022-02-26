﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    [Serializable]
    public class Macro : IList<MacroInstruction>
    {
        [Newtonsoft.Json.JsonProperty("Instructions")]
        private readonly List<MacroInstruction> _instructions = new List<MacroInstruction>();

        [Newtonsoft.Json.JsonIgnore]
        public MacroInstruction this[int index] { get => ((IList<MacroInstruction>)_instructions)[index]; set => ((IList<MacroInstruction>)_instructions)[index] = value; }

        [Newtonsoft.Json.JsonIgnore]
        public int Count => ((ICollection<MacroInstruction>)_instructions).Count;

        [Newtonsoft.Json.JsonIgnore]
        public bool IsReadOnly => false;

        [Newtonsoft.Json.JsonIgnore]
        public float TotalSeconds => _instructions.Sum(i => i.Seconds);

        [Newtonsoft.Json.JsonIgnore]
        public TimeSpan TotalDuration => TimeSpan.FromSeconds(TotalSeconds);

        public void Add(MacroInstruction item)
        {
            ((ICollection<MacroInstruction>)_instructions).Add(item);
        }

        public void Clear()
        {
            ((ICollection<MacroInstruction>)_instructions).Clear();
        }

        public bool Contains(MacroInstruction item)
        {
            return ((ICollection<MacroInstruction>)_instructions).Contains(item);
        }

        public void CopyTo(MacroInstruction[] array, int arrayIndex)
        {
            ((ICollection<MacroInstruction>)_instructions).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MacroInstruction> GetEnumerator()
        {
            return ((IEnumerable<MacroInstruction>)_instructions).GetEnumerator();
        }

        public int IndexOf(MacroInstruction item)
        {
            return ((IList<MacroInstruction>)_instructions).IndexOf(item);
        }

        public void Insert(int index, MacroInstruction item)
        {
            ((IList<MacroInstruction>)_instructions).Insert(index, item);
        }

        public bool Remove(MacroInstruction item)
        {
            return ((ICollection<MacroInstruction>)_instructions).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<MacroInstruction>)_instructions).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_instructions).GetEnumerator();
        }

        public static Macro Create(params MacroInstruction[] instructions)
        {
            Macro result = new Macro();
            result._instructions.AddRange(instructions ?? Array.Empty<MacroInstruction>());

            return result;
        }

        public override string ToString() => String.Join('\n', _instructions.Select(i => i.ToString()));
    }
}
