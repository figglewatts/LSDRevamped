using System;
using System.Globalization;
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
            if (string.IsNullOrEmpty(displayString.Trim()) && !string.IsNullOrEmpty(binding.effectivePath))
            {
                displayString =
                    binding.effectivePath.Split(new[] { '/' }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
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
