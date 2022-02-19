using System.Collections;
using System.Globalization;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandModel
{
    public class MacroInstruction : IList<IControllerInput>
    {
        private static readonly CultureInfo SECONDS_FORMAT_CULTURE = new CultureInfo("en-US");

        private readonly List<IControllerInput> _input = new List<IControllerInput>();
        private float _seconds;

        public IControllerInput this[int index] { get => ((IList<IControllerInput>)_input)[index]; set => ((IList<IControllerInput>)_input)[index] = value; }

        public float Seconds { 
            get => _seconds; 
            set => _seconds = Math.Abs(value); 
        }

        public int Count => ((ICollection<IControllerInput>)_input).Count;

        public bool IsReadOnly => false;

        public MacroInstruction(float seconds)
        {
            Seconds = seconds;
        }

        public MacroInstruction(float seconds, IEnumerable<IControllerInput> input)
        {
            Seconds = seconds;
            _input.AddRange(input);
        }

        public static MacroInstruction Create(float seconds, params IControllerInput[] inputParams)
        {
            return new MacroInstruction(seconds, inputParams ?? Array.Empty<IControllerInput>());
        }

        public void Add(IControllerInput item)
        {
            ((ICollection<IControllerInput>)_input).Add(item);
        }

        public void Clear()
        {
            ((ICollection<IControllerInput>)_input).Clear();
        }

        public bool Contains(IControllerInput item)
        {
            return ((ICollection<IControllerInput>)_input).Contains(item);
        }

        public void CopyTo(IControllerInput[] array, int arrayIndex)
        {
            ((ICollection<IControllerInput>)_input).CopyTo(array, arrayIndex);
        }

        public IEnumerator<IControllerInput> GetEnumerator()
        {
            return ((IEnumerable<IControllerInput>)_input).GetEnumerator();
        }

        public int IndexOf(IControllerInput item)
        {
            return ((IList<IControllerInput>)_input).IndexOf(item);
        }

        public void Insert(int index, IControllerInput item)
        {
            ((IList<IControllerInput>)_input).Insert(index, item);
        }

        public bool Remove(IControllerInput item)
        {
            return ((ICollection<IControllerInput>)_input).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<IControllerInput>)_input).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_input).GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(' ', _input.Select(i => i.Macro)) + " " + Seconds.ToString(".4", SECONDS_FORMAT_CULTURE) + "s";
        }
    }
}
