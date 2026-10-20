using System;
using System.Globalization;
using System.Linq;
using UnityEngine.InputSystem;

namespace LSDR.InputManagement
{
    public static class UnityInputExtensions
    {
        public static string GetBindingDisplayStringNotEmpty(this InputAction inputAction, InputBinding binding)
        {
            string displayString =
                inputAction.GetBindingDisplayString(binding,
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
            // some keyboard layouts give keys like space and tab a display name of only invisible characters
            bool visible = displayString.Any(c => !char.IsWhiteSpace(c) && !char.IsControl(c));
            if (!visible && !string.IsNullOrEmpty(binding.effectivePath))
            {
                displayString =
                    binding.effectivePath.Split(new[] { '/' }, count: 2, StringSplitOptions.RemoveEmptyEntries)[1];
            }
            displayString = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(displayString);

            if (string.IsNullOrWhiteSpace(displayString))
            {
                displayString = "<unbound>";
            }

            return displayString;
        }
    }
}
