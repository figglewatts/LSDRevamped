using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using LSDR.InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI.Settings
{
    // shows the current scheme's bindings for the device in use
    public class UIControlSchemePreview : MonoBehaviour
    {
        public ControlSchemeLoaderSystem ControlSchemeLoader;
        public Text HeadingText;
        public Text NamesText;
        public Text BindingsText;

        protected InputActions _inputActions;
        protected ControlScheme _shownScheme;
        protected string _shownSchemeString;
        protected bool _shownGamepad;

        public void OnEnable() { _shownScheme = null; }

        public void Update()
        {
            ControlScheme scheme = ControlSchemeLoader.Current;
            bool gamepad = ControlSchemeLoader.LastUsedGamepad;
            if (scheme == _shownScheme && scheme?.SchemeString == _shownSchemeString && gamepad == _shownGamepad) return;

            _shownScheme = scheme;
            _shownSchemeString = scheme?.SchemeString;
            _shownGamepad = gamepad;
            HeadingText.text = scheme != null ? gamepad ? "Gamepad controls" : "Keyboard and mouse controls" : "";
            if (scheme != null) describe(scheme, gamepad);
            else NamesText.text = BindingsText.text = "";
        }

        // separate columns so they line up
        protected void describe(ControlScheme scheme, bool gamepad)
        {
            if (_inputActions == null) _inputActions = new InputActions();
            _inputActions.RemoveAllBindingOverrides();
            if (!string.IsNullOrEmpty(scheme.SchemeString)) _inputActions.LoadBindingOverridesFromJson(scheme.SchemeString);

            var names = new StringBuilder();
            var bindings = new StringBuilder();
            foreach (InputAction action in _inputActions.Game.Get())
            {
                if (action.name == "Pause" || (action.name == "Look" && !scheme.FpsControls)) continue;

                List<string> actionBindings = bindingsFor(action, gamepad);
                if (actionBindings.Count == 0) continue;
                if (names.Length > 0)
                {
                    names.Append('\n');
                    bindings.Append('\n');
                }

                names.Append(spaceOut(action.name));
                bindings.Append(string.Join(", ", actionBindings));
            }

            NamesText.text = names.ToString();
            BindingsText.text = bindings.ToString();
        }

        protected static List<string> bindingsFor(InputAction action, bool gamepad)
        {
            var bindings = new List<string>();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                InputBinding binding = action.bindings[i];
                if (binding.isPartOfComposite) continue;

                // use the first part of a composite
                string path = binding.isComposite && i + 1 < action.bindings.Count
                    ? action.bindings[i + 1].effectivePath
                    : binding.effectivePath;
                if (string.IsNullOrEmpty(path) || isGamepadPath(path) != gamepad) continue;

                string display = action.GetBindingDisplayString(i);
                if (!string.IsNullOrEmpty(display)) bindings.Add(display);
            }

            return bindings;
        }

        // rebinds can save a specific gamepad layout
        protected static bool isGamepadPath(string path)
        {
            string layout = InputControlPath.TryGetDeviceLayout(path);
            return layout != null && InputSystem.IsFirstLayoutBasedOnSecond(layout, "Gamepad");
        }

        protected static string spaceOut(string name) { return Regex.Replace(name, "(?<=[a-z])(?=[A-Z])", " "); }
    }
}