using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    [Serializable]
    public class Macro : IList<IMacroInstruction>
    {
        [Newtonsoft.Json.JsonProperty("Instructions")]
        private readonly List<IMacroInstruction> _instructions = new List<IMacroInstruction>();

        [Newtonsoft.Json.JsonIgnore]
        public IMacroInstruction this[int index] { get => ((IList<IMacroInstruction>)_instructions)[index]; set => ((IList<IMacroInstruction>)_instructions)[index] = value; }

        [Newtonsoft.Json.JsonIgnore]
        public int Count => ((ICollection<IMacroInstruction>)_instructions).Count;

        [Newtonsoft.Json.JsonIgnore]
        public bool IsReadOnly => false;

        [Newtonsoft.Json.JsonIgnore]
        public float TotalSeconds => _instructions.Sum(i => i.Seconds);

        [Newtonsoft.Json.JsonIgnore]
        public TimeSpan TotalDuration => TimeSpan.FromSeconds(TotalSeconds);

        public void Add(IMacroInstruction item)
        {
            ((ICollection<IMacroInstruction>)_instructions).Add(item);
        }

        public void Clear()
        {
            ((ICollection<IMacroInstruction>)_instructions).Clear();
        }

        public bool Contains(IMacroInstruction item)
        {
            return ((ICollection<IMacroInstruction>)_instructions).Contains(item);
        }

        public void CopyTo(IMacroInstruction[] array, int arrayIndex)
        {
            ((ICollection<IMacroInstruction>)_instructions).CopyTo(array, arrayIndex);
        }

        public IEnumerator<IMacroInstruction> GetEnumerator()
        {
            return ((IEnumerable<IMacroInstruction>)_instructions).GetEnumerator();
        }

        public int IndexOf(IMacroInstruction item)
        {
            return ((IList<IMacroInstruction>)_instructions).IndexOf(item);
        }

        public void Insert(int index, IMacroInstruction item)
        {
            ((IList<IMacroInstruction>)_instructions).Insert(index, item);
        }

        public bool Remove(IMacroInstruction item)
        {
            return ((ICollection<IMacroInstruction>)_instructions).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<IMacroInstruction>)_instructions).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_instructions).GetEnumerator();
        }

        public static Macro Create(params IMacroInstruction[] instructions)
        {
            Macro result = new Macro();
            result._instructions.AddRange(instructions ?? Array.Empty<IMacroInstruction>());

            return result;
        }

        public override string ToString() => String.Join('\n', _instructions.Select(i => i.ToString()));
    }
}
