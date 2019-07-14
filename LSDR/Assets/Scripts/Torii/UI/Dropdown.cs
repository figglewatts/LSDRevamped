using System;
using System.Collections;
using System.Collections.Generic;
using Torii.Binding;
using UnityEngine;

namespace Torii.UI
{
    /// <summary>
    /// Specialized instancy of UnityEngine.UI.Dropdown with bindable value.
    /// </summary>
    public class Dropdown : UnityEngine.UI.Dropdown, IPropertyWatcher
    {

        public new int value
        {
            get { return base.value; }
            set
            {
                base.value = value;
                NotifyPropertyChange(nameof(value));
            }
        }

        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(i => { NotifyPropertyChange(nameof(value)); });
            GUID = Guid.NewGuid();
        }

        [HideInInspector] public event Action<string, IPropertyWatcher> OnPropertyChange;

        [HideInInspector] public Guid GUID { get; private set; }

        public void NotifyPropertyChange(string propertyName)
        {
            OnPropertyChange?.Invoke(propertyName, this);
        }
    }
}