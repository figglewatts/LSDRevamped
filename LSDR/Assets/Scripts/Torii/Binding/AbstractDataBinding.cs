using System;

namespace Torii.Binding
{
    public abstract class AbstractDataBinding
    {
        protected Delegate _bind;
        public string TargetReference { get; }

        public abstract void Invoke();

        protected AbstractDataBinding(string targetReference)
        {
            TargetReference = targetReference;
        }
    }
}