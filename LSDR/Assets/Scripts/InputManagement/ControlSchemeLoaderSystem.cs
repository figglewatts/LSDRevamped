using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace LSDR.InputManagement
{
    [CreateAssetMenu(menuName = "System/ControlSchemeLoaderSystem")]
    public class ControlSchemeLoaderSystem : ScriptableObject
    {
        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private string _controlSchemesPath;

        public Action<InputDevice> OnLastUsedDeviceChanged;
        public List<ControlScheme> Schemes;
        public InputActions InputActions { get; protected set; }
        public InputDevice LastUsedDevice { get; protected set; }

        public bool LastUsedGamepad => LastUsedDevice is Gamepad;

        public ControlScheme Current
        {
            get
            {
                if (_currentSchemeHandle > Schemes.Count)
                    return Schemes[Schemes.Count - 1];
                if (_currentSchemeHandle < 0) return Schemes[0];

                return Schemes[_currentSchemeHandle];
            }
        }

        protected int _currentSchemeHandle { get; private set; }

        public void OnEnable()
        {
            InputActions = new InputActions();
            _controlSchemesPath = PathUtil.Combine(Application.persistentDataPath, "new-input-schemes");
            InputSystem.onEvent += OnInputSystemEvent;
            InputSystem.onDeviceChange += OnInputSystemDeviceChange;
        }

        public void OnInputSystemEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (LastUsedDevice == device) return;

            if (eventPtr.type == StateEvent.Type)
            {
                // some devices spam events when they shouldn't - so lets check the controls
                // in the changed event and find if any changed above zero
                if (!eventPtr.EnumerateChangedControls(device, 0.0001f).Any()) return;
            }

            LastUsedDevice = device;
            OnLastUsedDeviceChanged?.Invoke(device);
        }

        public void OnInputSystemDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (LastUsedDevice == device) return;
            LastUsedDevice = device;
            OnLastUsedDeviceChanged?.Invoke(device);
        }

        public void LoadSchemes()
        {
            Debug.Log("Deserialising control schemes...");

            InputActions.RemoveAllBindingOverrides();

            Schemes = new List<ControlScheme>();

            EnsureDefaultSchemes();

            // if the directory doesn't exist, create it
            if (!Directory.Exists(_controlSchemesPath)) Directory.CreateDirectory(_controlSchemesPath);

            // deserialize all of the control schemes in the path
            foreach (string file in Directory.GetFiles(_controlSchemesPath, "*.dat"))
                CreateScheme(_serializer.Deserialize<ControlScheme>(file));
        }

        public void SaveSchemes()
        {
            Debug.Log("Serialising control schemes...");

            foreach (ControlScheme scheme in Schemes)
                _serializer.Serialize(scheme, PathUtil.Combine(_controlSchemesPath, scheme.Name + ".dat"));
        }

        public void EnsureDefaultSchemes()
        {
            bool schemeCreated = false;
            if (!File.Exists(PathUtil.Combine(_controlSchemesPath, "Classic.dat")))
            {
                Debug.Log("Unable to find 'Classic' control scheme - creating...");
                Schemes.Add(new ControlScheme("Classic", InputActions, false, 15F, false, false));
                schemeCreated = true;
            }

            if (!File.Exists(PathUtil.Combine(_controlSchemesPath, "Revamped.dat")))
            {
                Debug.Log("Unable to find 'Revamped' control scheme - creating...");
                Schemes.Add(new ControlScheme("Revamped", InputActions, true, 15F, false, false));
                schemeCreated = true;
            }

            if (schemeCreated)
            {
                SaveSchemes();

                // we now need to sort the list, as any schemes that might have already been there before the default ones
                // will now be before them in the list -- this is not preferable as loading them as normal will simply
                // load them in alphabetical order, so this is to emulate that
                Schemes.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
            }
        }

        public void SelectScheme(int idx)
        {
            if (idx < 0 || idx >= Schemes.Count)
            {
                Debug.LogError(
                    $"Cannot select scheme {idx}, as it's outside the bounds of the currently loaded schemes");
                return;
            }

            _currentSchemeHandle = idx;
            Assert.IsNotNull(Schemes[idx].SchemeString);
            InputActions.LoadBindingOverridesFromJson(Schemes[idx].SchemeString);
        }

        public void SelectScheme(ControlScheme scheme)
        {
            int idx = Schemes.IndexOf(scheme);
            if (idx == -1)
            {
                Debug.LogWarning(
                    $"Unable to select scheme '{scheme}': not found in schemes list (is it created?)");
                return;
            }

            SelectScheme(idx);
        }

        public void CreateScheme(ControlScheme scheme, bool select = false)
        {
            Assert.IsNotNull(scheme.SchemeString);

            Schemes.Add(scheme);

            if (select) SelectScheme(scheme);
        }
    }
}
