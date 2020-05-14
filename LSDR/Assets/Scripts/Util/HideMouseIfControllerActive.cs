using System;
using InControl;
using UnityEngine;

namespace LSDR.Util
{
    public class HideMouseIfControllerActive : MonoBehaviour
    {
        private Vector3 lastMousePos;
        
        public void Start()
        {
            checkActiveDeviceController(InputManager.ActiveDevice);
            InputManager.OnActiveDeviceChanged += checkActiveDeviceController;
            InputManager.OnDeviceDetached += checkActiveDeviceController;
            lastMousePos = Input.mousePosition;
        }

        public void Update()
        {
            reactivateMouseIfMoved();
        }

        private void reactivateMouseIfMoved()
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePos;
            if (Cursor.visible == false && mouseDelta.sqrMagnitude > 1)
            {
                Cursor.visible = true;
            }

            lastMousePos = Input.mousePosition;
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
