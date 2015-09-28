using System;

namespace MemCache
{
    public interface IFreezable
    {
        void Freeze();
    }

    public abstract class FreezableBase : IFreezable
    {
        private bool _isFrozen;

        public void Freeze()
        {
            _isFrozen = true;
        }

        protected void CheckIfFrozen()
        {
            if (_isFrozen)
                throw new InvalidOperationException("Attempted to modify a frozen instance");
        }
    }
}