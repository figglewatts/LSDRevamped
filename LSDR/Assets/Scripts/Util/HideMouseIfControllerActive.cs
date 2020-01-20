using System;
using InControl;
using UnityEngine;

namespace LSDR.Util
{
    public class HideMouseIfControllerActive : MonoBehaviour
    {
        public void Start()
        {
            checkActiveDeviceController(InputManager.ActiveDevice);
            InputManager.OnActiveDeviceChanged += checkActiveDeviceController;
            InputManager.OnDeviceDetached += checkActiveDeviceController;
        }

        private bool deviceIsController(InputDevice device)
        {
            return !device.Name.Equals("None", StringComparison.InvariantCulture);
        }

        private void checkActiveDeviceController(InputDevice device)
        {
            Cursor.visible = !deviceIsController(InputManager.ActiveDevice);
        }

        private void OnDestroy()
        {
            InputManager.OnActiveDeviceChanged -= checkActiveDeviceController;
            InputManager.OnDeviceDetached -= checkActiveDeviceController;
        }
    }
}
