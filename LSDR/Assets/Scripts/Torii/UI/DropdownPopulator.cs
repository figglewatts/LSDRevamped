using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.UI
{
    public class DropdownPopulator : MonoBehaviour
    {
        protected Dropdown _dropdown;

        // Use this for initialization
        protected virtual void Awake() { _dropdown = GetComponent<Dropdown>(); }

        public void Clear()
        {
            _dropdown.ClearOptions();
        }

        public void Populate(List<string> values)
        {
            Clear();
            _dropdown.AddOptions(values);
        }
    }
}