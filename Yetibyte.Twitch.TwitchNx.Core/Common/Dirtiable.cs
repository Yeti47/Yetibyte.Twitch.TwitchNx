namespace Yetibyte.Twitch.TwitchNx.Core.Common
{
    public abstract class Dirtiable : IDirtiable
    {
        public virtual bool IsDirty { get; protected set; }


        public event EventHandler? IsDirtyChanged;

        public virtual void MarkDirty()
        {
            IsDirty = true;
        }

        public virtual void UnmarkDirty()
        {
            IsDirty = false;
        }

        protected virtual void OnIsDirtyChanged()
        {
            var handler = IsDirtyChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
