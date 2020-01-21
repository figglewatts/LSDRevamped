using System;
using InControl;
using UnityEngine;

namespace LSDR.Util
{
    [RequireComponent(typeof(InControlInputModule))]
    public class InControlInputModuleController : MonoBehaviour
    {
        private InControlInputModule _inputModule;

        public void Start()
        {
            _inputModule = GetComponent<InControlInputModule>();
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
            _inputModule.allowMouseInput = !deviceIsController(InputManager.ActiveDevice);
        }

        private void OnDestroy()
        {
            InputManager.OnActiveDeviceChanged -= checkActiveDeviceController;
            InputManager.OnDeviceDetached -= checkActiveDeviceController;
        }
    }
}
