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

    /// <summary>
    ///     BindBroker is a class used for being the 'middleman' between data that is bound together.
    ///     You can register data stored in classes which implement the IPropertyWatcher interface, then create bindings
    ///     between properties of these classes, so that when one changes the other changes to match it, and optionally
    ///     vice-versa.
    ///     This is quite a powerful concept, and it used extensively for matching UI element values to internal
    ///     gameplay values (for example in the settings menu).
    /// </summary>
    public class BindBroker
    {
        // store the bindings
        private readonly Dictionary<string, List<AbstractDataBinding>> _bindings;

        // we need to keep track of changes being handled so we don't get into an endless loop of updates
        private readonly Stack<string> _changesBeingHandled;

        /// <summary>
        ///     Create a new BindBroker.
        /// </summary>
        public BindBroker()
        {
            _bindings = new Dictionary<string, List<AbstractDataBinding>>();
            _changesBeingHandled = new Stack<string>();
        }

        /// <summary>
        ///     Register a data class with this BindBroker. You need to register all classes that will be using data
        ///     binding, as otherwise the BindBroker won't know to update values when one changes.
        /// </summary>
        /// <param name="watcher">Instance of the class (implementing IPropertyWatcher) to bind.</param>
        public void RegisterData(IPropertyWatcher watcher) { watcher.OnPropertyChange += handleChange; }

        /// <summary>
        ///     Deregister a data class with this BindBroker. This is the inverse of <c>RegisterData()</c>.
        /// </summary>
        /// <param name="watcher">Instance of the class (implementing IPropertyWatcher) to unbind.</param>
        public void DeregisterData(IPropertyWatcher watcher) { watcher.OnPropertyChange -= handleChange; }

        /// <summary>
        ///     Bind a piece of data from one PropertyWatcher to another.
        ///     Bind the value of a volume slider to a music volume value, and vice-versa:
        ///     <c>Bind&lt;float&gt;(() => MusicVolumeSlider.value, () => CurrentSettings.MusicVolume, BindingType.TwoWay)</c>
        /// </summary>
        /// <param name="binder">
        ///     An expression for the 'binder' value. Must be a property of a class that
        ///     implements IPropertyWatcher.
        /// </param>
        /// <param name="bindee">
        ///     An expression for the 'bindee' value. Must be a property of a class that
        ///     implements IPropertyWatcher.
        /// </param>
        /// <param name="bindingType">
        ///     What type of binding this is. Can be one way (to set bindee to binder when
        ///     binder value changes) or two way (to set in either direction).
        /// </param>
        /// <typeparam name="TType">The type of the value we're binding. Can usually be inferred.</typeparam>
        public void Bind<TType>(Expression<Func<TType>> binder, Expression<Func<TType>> bindee,
            BindingType bindingType)
        {
            // get reference to the binder
            MemberExpression binderMemberExp = (MemberExpression)binder.Body;
            IPropertyWatcher binderInstance =
                Expression.Lambda<Func<IPropertyWatcher>>(binderMemberExp.Expression).Compile()();
            string binderReference = makePropertyReference(binderInstance.GUID, binderMemberExp.Member.Name);

            // get reference to the bindee
            MemberExpression bindeeMemberExp = (MemberExpression)bindee.Body;
            IPropertyWatcher bindeeInstance =
                Expression.Lambda<Func<IPropertyWatcher>>(bindeeMemberExp.Expression).Compile()();
            string bindeeReference = makePropertyReference(bindeeInstance.GUID, bindeeMemberExp.Member.Name);

            // create binder->bindee binding
            var binding = new DataBinding<TType>(binder, bindee, bindeeReference);
            createBinding(binderReference, binding);

            // if it's two way, create bindee->binder binding
            if (bindingType == BindingType.TwoWay)
            {
                var oppositeBinding = new DataBinding<TType>(bindee, binder, binderReference);
                createBinding(bindeeReference, oppositeBinding);

            }
        }

        // this is the callback that is called when a bound value in an IPropertyWatcher is changed
        private void handleChange(string propertyName, IPropertyWatcher instance)
        {
            List<AbstractDataBinding> bindingList;
            string propertyReference = makePropertyReference(instance.GUID, propertyName);

            if (_bindings.TryGetValue(propertyReference, out bindingList))
            {
                // we're now handling this change, add it to the stack
                _changesBeingHandled.Push(propertyReference);

                foreach (AbstractDataBinding binding in bindingList)
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

        // create a binding
        private void createBinding(string reference, AbstractDataBinding binding)
        {
            List<AbstractDataBinding> bindingList;
            if (_bindings.TryGetValue(reference, out bindingList))
            {
                bindingList.Add(binding);
            }
            else
            {
                _bindings[reference] = new List<AbstractDataBinding>(new[] { binding });
            }
        }

        // a property reference is the GUID of the class instance the property is from coupled with the property name
        // they are separated by a '.' -- a property reference allows the bind broker to recognise which changes come
        // from what instance
        private string makePropertyReference(Guid instance, string propertyName)
        {
            return instance + "." + propertyName;
        }
    }
}
