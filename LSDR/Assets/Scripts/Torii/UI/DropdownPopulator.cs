using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.UI
{
    /// <summary>
    /// Populate a dropdown component with some strings.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class DropdownPopulator : MonoBehaviour, IPopulator<string>
    {
        protected Dropdown _dropdown;

        // Use this for initialization
        protected virtual void Awake() { _dropdown = GetComponent<Dropdown>(); }

        public void Clear()
        {
            _dropdown.ClearOptions();
        }

        public void Populate(List<string> with)
        {
            Clear();
            _dropdown.AddOptions(with);
        }
    }
}