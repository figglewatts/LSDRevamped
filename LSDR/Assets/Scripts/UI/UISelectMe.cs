using System.Collections;
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

        private void Start() { StartCoroutine(waitFrameThenSelect()); }

        private void OnEnable()
        {
            StartCoroutine(waitFrameThenSelect());
            ControlScheme.OnLastUsedDeviceChanged += onDeviceChanged;
        }

        private void OnDisable() { ControlScheme.OnLastUsedDeviceChanged -= onDeviceChanged; }

        private void onDeviceChanged(InputDevice device)
        {
            if (device is Gamepad) StartCoroutine(waitFrameThenSelect());
        }

        private IEnumerator waitFrameThenSelect()
        {
            yield return null;
            if (ControlScheme.LastUsedGamepad || SelectEvenWithMouse) GetComponent<Selectable>().Select();
        }
    }
}
