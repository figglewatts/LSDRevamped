using System;
using Torii.Binding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Torii.UI
{
    /// <summary>
    ///     Specialized instance of UnityEngine.UI.Slider with bindable value.
    /// </summary>
    public class Slider : UnityEngine.UI.Slider, IPropertyWatcher
    {
        // snap increment, 0 for smooth
        public float Step;

        public new float value
        {
            get => base.value;
            set
            {
                base.value = value;
                NotifyPropertyChange(nameof(value));
            }
        }

        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(f => { NotifyPropertyChange(nameof(value)); });
            GUID = Guid.NewGuid();
        }

        protected override void Set(float input, bool sendCallback = true)
        {
            if (Step > 0) input = (float)Math.Round(Mathf.Round(input / Step) * Step, 4);
            base.Set(input, sendCallback);
        }

        public override void OnMove(AxisEventData eventData)
        {
            bool horizontal = direction == Direction.LeftToRight || direction == Direction.RightToLeft;
            bool left = eventData.moveDir == MoveDirection.Left;
            bool sideways = left || eventData.moveDir == MoveDirection.Right;
            if (Step <= 0 || !horizontal || !sideways || !IsActive() || !IsInteractable() ||
                (left ? FindSelectableOnLeft() : FindSelectableOnRight()) != null)
            {
                base.OnMove(eventData);
                return;
            }

            bool decrease = left == (direction == Direction.LeftToRight);
            Set(base.value + (decrease ? -Step : Step));
        }

        [HideInInspector] public event Action<string, IPropertyWatcher> OnPropertyChange;

        [HideInInspector] public Guid GUID { get; private set; }

        public void NotifyPropertyChange(string propertyName)
        {
            OnPropertyChange?.Invoke(propertyName, this);
        }
    }
}
