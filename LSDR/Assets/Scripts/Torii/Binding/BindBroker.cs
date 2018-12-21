using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Torii.Binding
{
    public enum BindingType
    {
        OneWay,
        TwoWay
    }

    public class BindBroker
    {
        private readonly Dictionary<string, List<AbstractDataBinding>> _bindings;
        private readonly Stack<string> _changesBeingHandled;

        public BindBroker()
        {
            _bindings = new Dictionary<string, List<AbstractDataBinding>>();
            _changesBeingHandled = new Stack<string>();
        }

        public void RegisterData(IPropertyWatcher watcher) { watcher.OnPropertyChange += handleChange; }

        public void DeregisterData(IPropertyWatcher watcher) { watcher.OnPropertyChange -= handleChange; }

        public void Bind<TType>(Expression<Func<TType>> binder, Expression<Func<TType>> bindee, BindingType bindingType)
        {
            var binderMemberExp = (MemberExpression)binder.Body;
            IPropertyWatcher binderInstance = Expression.Lambda<Func<IPropertyWatcher>>(binderMemberExp.Expression).Compile()();
            string binderReference = makePropertyReference(binderInstance.GUID, binderMemberExp.Member.Name);

            var bindeeMemberExp = (MemberExpression)bindee.Body;
            IPropertyWatcher bindeeInstance = Expression.Lambda<Func<IPropertyWatcher>>(bindeeMemberExp.Expression).Compile()();
            string bindeeReference = makePropertyReference(bindeeInstance.GUID, bindeeMemberExp.Member.Name);

            DataBinding<TType> binding = new DataBinding<TType>(binder, bindee, bindeeReference);
            createBinding(binderReference, binding);

            if (bindingType == BindingType.TwoWay)
            {
                DataBinding<TType> oppositeBinding = new DataBinding<TType>(bindee, binder, binderReference);
                createBinding(bindeeReference, oppositeBinding);

            }
        }

        private void handleChange(string propertyName, IPropertyWatcher instance)
        {
            List<AbstractDataBinding> bindingList;
            string propertyReference = makePropertyReference(instance.GUID, propertyName);

            if (_bindings.TryGetValue(propertyReference, out bindingList))
            {
                // we're now handling this change, add it to the stack
                _changesBeingHandled.Push(propertyReference);

                foreach (var binding in bindingList)
                {

                    if (_changesBeingHandled.Contains(binding.TargetReference))
                    {
                        // the Target of this change is one that we've already handled,
                        // so it will be no use setting it again
                        continue;
                    }

                    binding.Invoke();
                }

                // we've finished handling this change
                _changesBeingHandled.Pop();
            }
        }

        private void createBinding(string reference, AbstractDataBinding binding)
        {
            List<AbstractDataBinding> bindingList;
            if (_bindings.TryGetValue(reference, out bindingList))
            {
                bindingList.Add(binding);
            }
            else
            {
                _bindings[reference] = new List<AbstractDataBinding>(new [] { binding });
            }
        }

        // a property reference is the GUID of the class instance the property is from coupled with the property name
        // they are separated by a '.' -- a property reference allows the bind broker to recognise which changes come
        // from what instance
        private string makePropertyReference(Guid instance, string propertyName)
        {
            return instance.ToString() + "." + propertyName;
        }
    }
}