using System;
using Torii.Binding;
using UnityEngine;

namespace Torii.UI
{
    /// <summary>
    ///     Specialized instance of UnityEngine.UI.Toggle with bindable value.
    /// </summary>
    public class Toggle : UnityEngine.UI.Toggle, IPropertyWatcher
    {
        public new bool isOn
        {
            get => base.isOn;
            set
            {
                base.isOn = value;
                NotifyPropertyChange(nameof(isOn));
            }
        }

        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(v => { NotifyPropertyChange(nameof(isOn)); });
            GUID = Guid.NewGuid();
        }

        [HideInInspector]
        public event Action<string, IPropertyWatcher> OnPropertyChange;

        [HideInInspector]
        public Guid GUID { get; private set; }

        public void NotifyPropertyChange(string propertyName)
        {
            OnPropertyChange?.Invoke(propertyName, this);
        }
    }
}
