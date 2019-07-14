using System;

namespace Torii.Binding
{
    /// <summary>
    /// The IPropertyWatcher interface defines functions and properties for notifying when a property is changed.
    ///
    /// NotifyPropertyChange needs to be defined like this:
    /// <code>
    /// public void NotifyPropertyChange(string propertyName)
    /// {
    ///     OnPropertyChange?.Invoke(propertyName, this);
    /// }
    /// </code>
    /// 
    /// Properties that are able to be bound need to be defined like this:
    /// <code>
    /// private float _value;
    /// 
    /// public float value
    /// {
    ///     get { return _value; }
    ///     set
    ///     {
    ///         _value = value;
    ///         NotifyPropertyChange(nameof(value));
    ///     }
    /// }
    /// </code>
    /// Otherwise the notification of a modification won't be fired.
    ///
    /// Additionally, in the constructor you must assign a GUID:
    /// <code>
    /// public WatchedData()
    /// {
    ///     GUID = Guid.NewGuid();
    /// }
    /// </code>
    /// </summary>
    public interface IPropertyWatcher
    {
        /// <summary>
        /// This event is fired when a property is changed.
        /// </summary>
        event Action<string, IPropertyWatcher> OnPropertyChange;

        /// <summary>
        /// Used to refer to this specific instance.
        /// </summary>
        Guid GUID { get; }

        /// <summary>
        /// Invoke the property change event. Should be called whenever a property is changed in any way.
        /// </summary>
        /// <param name="propertyName">The name of the property. Use nameof() wherever possible.</param>
        void NotifyPropertyChange(string propertyName);
    }
}