using System;
using System.Collections;
using System.Collections.Generic;
using Torii.Binding;
using UnityEngine;

namespace Torii.UI
{
    public class Text : UnityEngine.UI.Text, IPropertyWatcher
    {
        public new string text
        {
            get { return base.text; }
            set
            {
                base.text = value;
                NotifyPropertyChange(nameof(text));
            }
        }

        protected override void Awake()
        {
            base.Awake();
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