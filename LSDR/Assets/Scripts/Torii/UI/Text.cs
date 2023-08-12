﻿using System;
using Torii.Binding;
using UnityEngine;

namespace Torii.UI
{
    /// <summary>
    ///     Specialized instance of UnityEngine.UI.Text with bindable text.
    /// </summary>
    public class Text : UnityEngine.UI.Text, IPropertyWatcher
    {
        public new string text
        {
            get => base.text;
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
