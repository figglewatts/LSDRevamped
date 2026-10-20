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
        protected const float GAMEPAD_DEAD_ZONE = 0.2f;

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
                if (_currentSchemeHandle < 0) return Schemes[index: 0];

                return Schemes[_currentSchemeHandle];
            }
        }

        public int CurrentSchemeIndex => _currentSchemeHandle;

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
            if (LastUsedDevice == device || !isPlayerDevice(device)) return;
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

            // some devices spam events when they shouldn't - so lets check the controls in the changed
            // event and only count it if one really moved, past a dead zone for gamepads
            float threshold = device is Gamepad ? GAMEPAD_DEAD_ZONE : 0.0001f;
            if (!eventPtr.EnumerateChangedControls(device, magnitudeThreshold: threshold).Any()) return;

            setLastUsedDevice(device);
        }

        public void OnInputSystemDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (LastUsedDevice == device || !isPlayerDevice(device)) return;
            if (change != InputDeviceChange.Added && change != InputDeviceChange.Reconnected) return;
            setLastUsedDevice(device);
        }

        // only the devices people play with count, not things like a controller's motion sensors
        protected static bool isPlayerDevice(InputDevice device) =>
            device is Mouse || device is Keyboard || device is Gamepad;

        protected void setLastUsedDevice(InputDevice device)
        {
            Debug.Log($"Input switched to {device.displayName}");
            LastUsedDevice = device;
            OnLastUsedDeviceChanged?.Invoke(device);
        }

        public void LoadSchemes()
        {
            Debug.Log("Deserialising control schemes...");

            InputActions.RemoveAllBindingOverrides();

            Schemes = new List<ControlScheme>();

            ensureDirectory();

            // deserialize all of the control schemes in the path
            foreach (string file in Directory.GetFiles(_controlSchemesPath, "*.dat"))
                CreateScheme(_serializer.Deserialize<ControlScheme>(file));

            EnsureDefaultSchemes();
        }

        public void SaveSchemes()
        {
            Debug.Log("Serialising control schemes...");

            ensureDirectory();

            foreach (ControlScheme scheme in Schemes)
                _serializer.Serialize(scheme, getControlSchemePath(scheme));
        }

        protected string getControlSchemePath(ControlScheme scheme)
        {
            return PathUtil.Combine(_controlSchemesPath, scheme.Name + ".dat");
        }

        public void EnsureDefaultSchemes()
        {
            // built in schemes can't be edited, so rebuild them in case their bindings changed.
            // classic uses the d-pad and south to run, revamped uses the sticks and L3
            Schemes.Add(createDefaultScheme("Classic", fpsControls: false, disabledMove: "<Gamepad>/leftStick",
                gamepadRun: "<Gamepad>/buttonSouth"));
            Schemes.Add(createDefaultScheme("Revamped", fpsControls: true, disabledMove: "<Gamepad>/dpad"));
            SaveSchemes();

            // sort the list to match the alphabetical order the schemes would load in
            Schemes.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
        }

        protected ControlScheme createDefaultScheme(string name, bool fpsControls, string disabledMove,
            string gamepadRun = null)
        {
            ControlScheme saved = Schemes.Find(s => s.Name == name && !s.Editable);
            if (saved != null) Schemes.Remove(saved);

            InputActions.RemoveAllBindingOverrides();
            InputActions.Game.Move.ApplyBindingOverride(new InputBinding { path = disabledMove, overridePath = "" });
            if (gamepadRun != null)
            {
                InputActions.Game.Run.ApplyBindingOverride(
                    new InputBinding { path = "<Gamepad>/leftStickPress", overridePath = gamepadRun });
            }
            var scheme = new ControlScheme(name, InputActions, fpsControls,
                mouseSensitivity: saved?.MouseSensitivity ?? 15F, invertLookY: saved?.InvertLookY ?? false,
                editable: false);
            InputActions.RemoveAllBindingOverrides();
            return scheme;
        }

        public void SelectScheme(int idx)
        {
            if (idx < 0 || idx >= Schemes.Count)
            {
                Debug.LogWarning(
                    $"Cannot select scheme {idx}, as it's outside the bounds of the currently loaded schemes");
                idx = 0;
            }

            Debug.Log($"Selecting scheme {idx}");
            _currentSchemeHandle = idx;
            Assert.IsNotNull(Schemes[idx].SchemeString);
            InputActions.LoadBindingOverridesFromJson(Schemes[idx].SchemeString);

            if (Current.InvertLookY) invertLook();
            else uninvertLook();

            InputActions.Enable();
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
            Schemes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCulture));

            if (select) SelectScheme(scheme);
        }

        public void DeleteScheme(ControlScheme scheme)
        {
            int idx = Schemes.IndexOf(scheme);
            if (idx == -1)
            {
                Debug.LogWarning(
                    $"Unable to delete scheme '{scheme}': not found in schemes list (is it created?)");
                return;
            }

            File.Delete(getControlSchemePath(scheme));
            Schemes.RemoveAt(idx);

            if (idx == _currentSchemeHandle && idx == Schemes.Count)
            {
                // we removed the last scheme, set current to new last scheme
                //_currentSchemeHandle = Schemes.Count - 1;
            }
        }

        protected void ensureDirectory()
        {
            // if the directory doesn't exist, create it
            if (!Directory.Exists(_controlSchemesPath)) Directory.CreateDirectory(_controlSchemesPath);
        }

        protected void invertLook()
        {
            InputActions.Game.Look.ApplyBindingOverride(
                new InputBinding { overrideProcessors = "invertVector2(invertX=false,invertY=true)" });
        }

        protected void uninvertLook()
        {
            InputActions.Game.Look.ApplyBindingOverride(
                new InputBinding { overrideProcessors = "invertVector2(invertX=false,invertY=false)" });
        }
    }
}
