using UnityEngine;
using UnityEngine.InputSystem;

namespace Torii.Console
{
    public class ConsoleOpener : MonoBehaviour
    {
        public const Key ConsoleKey = Key.Backquote;
        public UIDevConsole Console;

        public void Update()
        {
            if (Keyboard.current[ConsoleKey].wasPressedThisFrame)
            {
                Console.ToggleVisible();
            }
        }
    }
}
