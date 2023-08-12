using System;
using System.Linq.Expressions;

namespace Torii.Binding
{
    /// <summary>
    ///     A class to store a data binding between values of type T.
    /// </summary>
    /// <typeparam name="T">The type of the data being bound.</typeparam>
    public class DataBinding<T> : AbstractDataBinding
    {
        /// <summary>
        ///     Create a new data binding between two values.
        /// </summary>
        /// <param name="binder">Expression returning the binder.</param>
        /// <param name="bindee">Expression returning the bindee.</param>
        /// <param name="targetReference">Reference to the targer (GUID).</param>
        public DataBinding(Expression<Func<T>> binder, Expression<Func<T>> bindee, string targetReference)
            : base(targetReference)
        {
            BinaryExpression binderToBindee = Expression.Assign(bindee.Body, binder.Body);
            LambdaExpression binderToBindeeLambda = Expression.Lambda(binderToBindee);
            _bind = binderToBindeeLambda.Compile();
        }

        /// <summary>
        ///     Invoke this binding, setting the binder to the bindee
        /// </summary>
        public override void Invoke()
        {
            _bind.DynamicInvoke();
        }
    }
}
