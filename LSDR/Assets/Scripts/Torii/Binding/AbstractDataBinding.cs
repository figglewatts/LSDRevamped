using System;

namespace Torii.Binding
{
    /// <summary>
    ///     A class to store an AbstractDataBinding (any type). Concrete bindings will be instances of a generic subclass.
    /// </summary>
    public abstract class AbstractDataBinding
    {
        // the function that, when invoked, will perform the set operation of the binding
        protected Delegate _bind;

        /// <summary>
        ///     Create a new binding, giving the target GUID.
        /// </summary>
        /// <param name="targetReference">The target GUID.</param>
        protected AbstractDataBinding(string targetReference)
        {
            TargetReference = targetReference;
        }

        /// <summary>
        ///     The GUID of the target.
        /// </summary>
        public string TargetReference { get; }

        /// <summary>
        ///     Perform the set operation of this binding.
        /// </summary>
        public abstract void Invoke();
    }
}
