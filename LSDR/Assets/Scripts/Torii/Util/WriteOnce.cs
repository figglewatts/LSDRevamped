using System;

namespace Torii.Util
{
    /// <summary>
    /// Only write to a value once.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class WriteOnce<T>
    {
        private bool _hasBeenSet = false;

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (!_hasBeenSet)
                {
                    _value = value;
                    _hasBeenSet = true;
                }
                else
                {
                    throw new InvalidOperationException("Value has already been initialized!");
                }
            }
        }
    }
}