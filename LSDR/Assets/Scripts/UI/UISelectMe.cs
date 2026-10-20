using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSDR.InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI
{
    /// <summary>
    ///     When attached to a UI object, makes it so that this object is initially selected.
    /// </summary>
    public class UISelectMe : MonoBehaviour
    {
        public bool SelectEvenWithMouse;
        public ControlSchemeLoaderSystem ControlScheme;

        // most recently enabled wins
        private static readonly List<UISelectMe> _active = new List<UISelectMe>();

        public static UISelectMe CurrentDefault => _active.LastOrDefault(s => s.GetComponent<Selectable>().IsInteractable());

        private void Start() { StartCoroutine(waitFrameThenSelect()); }

        private void OnEnable()
        {
            _active.Add(this);
            StartCoroutine(waitFrameThenSelect());
        }

        private void OnDisable() { _active.Remove(this); }

        private IEnumerator waitFrameThenSelect()
        {
            yield return null;
            if (!(ControlScheme.LastUsedDevice is Mouse) || SelectEvenWithMouse) GetComponent<Selectable>().Select();
        }
    }
}
