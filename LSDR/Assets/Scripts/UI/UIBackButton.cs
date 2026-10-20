using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
    // cancel presses the most recently enabled back button
    [RequireComponent(typeof(Button))]
    public class UIBackButton : MonoBehaviour
    {
        private static readonly List<UIBackButton> _active = new List<UIBackButton>();

        public static Button Current => _active.Select(b => b.GetComponent<Button>())
                                               .LastOrDefault(b => b.IsInteractable());

        private void OnEnable() { _active.Add(this); }

        private void OnDisable() { _active.Remove(this); }
    }
}