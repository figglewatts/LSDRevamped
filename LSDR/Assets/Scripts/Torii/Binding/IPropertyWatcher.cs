using System;

namespace Torii.Binding
{
    public interface IPropertyWatcher
    {
        event Action<string, IPropertyWatcher> OnPropertyChange;

        Guid GUID { get; }

        void NotifyPropertyChange(string propertyName);
    }
}