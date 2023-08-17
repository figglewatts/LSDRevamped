using System;

namespace LSDR.SDK.Util
{
    public abstract class AbstractTypeManager<T> where T : class
    {
        protected static T _managed;

        public static T Managed
        {
            get
            {
                if (_managed == null)
                    throw new InvalidOperationException($"{typeof(T)} has not yet been provided to manager");

                return _managed;
            }
        }

        public static void ProvideManaged(T instance)
        {
            if (_managed != null)
                throw new InvalidOperationException($"{typeof(T)} has already been provided to manager");
            _managed = instance;
        }
    }
}
