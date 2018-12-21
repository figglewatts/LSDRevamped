using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Torii.Binding
{
    public class DataBinding<T> : AbstractDataBinding
    {
        public DataBinding(Expression<Func<T>> binder, Expression<Func<T>> bindee, string targetReference)
            : base(targetReference)
        {
            var binderToBindee = Expression.Assign(bindee.Body, binder.Body);
            var binderToBindeeLambda = Expression.Lambda(binderToBindee);
            _bind = binderToBindeeLambda.Compile();
        }

        public override void Invoke()
        {
            _bind.DynamicInvoke();
        }
    }
}