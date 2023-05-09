using UnityEngine;
using UnityEngine.InputSystem;

namespace LSDR.UI
{
    public class UIToggleOnKeyPress : MonoBehaviour
    {
        public GameObject Target;
        public Key Key;

        public void Update()
        {
            if (Keyboard.current[Key].wasPressedThisFrame) Target.SetActive(!Target.activeSelf);
        }
    }
}
