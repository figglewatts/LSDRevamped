using LSDR.SDK.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.SDK.DreamControl
{
    public class TextSpecialDayControl : MonoBehaviour
    {
        public Text Text;
        protected bool _over = false;

        public void BeginTextDay(TextSpecialDay day)
        {
            Text.text = day.Text;
        }

        public void Update()
        {
            bool skipInputPressed = Gamepad.current != null && (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                                                                Gamepad.current.startButton.wasPressedThisFrame);
            bool skipKeyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            if (!_over && (skipInputPressed || skipKeyPressed))
            {
                endTextDay();
            }
        }

        protected void endTextDay()
        {
            _over = true;
            DreamControlManager.Managed.EndDream();
        }
    }
}
