using LSDR.InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSDR.Util
{
    public class HideMouseIfControllerActive : MonoBehaviour
    {
        public ControlSchemeLoaderSystem ControlScheme;

        public void OnEnable()
        {
            checkActiveDeviceController(ControlScheme.LastUsedDevice);
            ControlScheme.OnLastUsedDeviceChanged += checkActiveDeviceController;
        }

        private void OnDisable()
        {
            if (ControlScheme != null) ControlScheme.OnLastUsedDeviceChanged -= checkActiveDeviceController;
        }

        private void checkActiveDeviceController(InputDevice device) { Cursor.visible = device is Mouse; }
    }
}
