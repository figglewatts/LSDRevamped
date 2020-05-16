using UnityEngine;

namespace Torii.Util
{
    public static class ToriiCursor
    {
        public static void Hide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void Show()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
