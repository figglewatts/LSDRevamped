using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InControl;
using LSDR.Util;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;

namespace LSDR.InputManagement
{
    [CreateAssetMenu(menuName="System/ControlSchemeLoaderSystem")]
    public class ControlSchemeLoaderSystem : ScriptableObject
    {
        public List<ControlScheme> Schemes;

        public ControlScheme Current
        {
            get
            {
                if (_currentSchemeHandle > Schemes.Count)
                {
                    return Schemes[Schemes.Count - 1];
                }
                else if (_currentSchemeHandle < 0)
                {
                    return Schemes[0];
                }

                return Schemes[_currentSchemeHandle];
            }
        }
        public int CurrentSchemeIndex => _currentSchemeHandle;

        private string _controlSchemesPath;
        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private int _currentSchemeHandle = 0;

        public void OnEnable()
        {
            _controlSchemesPath = PathUtil.Combine(Application.persistentDataPath, "input-schemes");

            InputManager.OnDeviceAttached += initInputDevice;
        }

        public void LoadSchemes()
        {
            Debug.Log("Deserialising control schemes...");

            Schemes = new List<ControlScheme>();
            
            EnsureDefaultSchemes();

            // if the directory doesn't exist, create it
            if (!Directory.Exists(_controlSchemesPath))
            {
                Directory.CreateDirectory(_controlSchemesPath);
            }

            // deserialize all of the control schemes in the path
            foreach (string file in Directory.GetFiles(_controlSchemesPath, "*.dat"))
            {
                Schemes.Add(_serializer.Deserialize<ControlScheme>(file));
            }
        }

        public void SaveSchemes()
        {
            Debug.Log("Serialising control schemes...");
            
            foreach (var scheme in Schemes)
            {
                _serializer.Serialize(scheme, PathUtil.Combine(_controlSchemesPath, scheme.Name + ".dat"));
            }
        }

        public void EnsureDefaultSchemes()
        {
            bool schemeCreated = false;
            if (!File.Exists(PathUtil.Combine(_controlSchemesPath, "Classic.dat")))
            {
                Debug.Log("Unable to find 'Classic' control scheme - creating...");
                Schemes.Add(new ControlScheme(ControlActions.CreateDefaultTank(), "Classic", false));
                schemeCreated = true;
            }

            if (!File.Exists(PathUtil.Combine(_controlSchemesPath, "Revamped.dat")))
            {
                Debug.Log("Unable to find 'Revamped' control scheme - creating...");
                Schemes.Add(new ControlScheme(ControlActions.CreateDefaultFps(), "Revamped", true, 15F));
                schemeCreated = true;
            }

            if (schemeCreated)
            {
                SaveSchemes();
                
                // we now need to sort the list, as any schemes that might have already been there before the default ones
                // will now be before them in the list -- this is not preferable as loading them as normal will simply
                // load them in alphabetical order, so this is to emulate that
                Schemes.Sort((x, y) => String.Compare(x.Name, y.Name, StringComparison.Ordinal));
            }
        }

        public void SelectScheme(int idx)
        {
            if (idx < 0 || idx >= Schemes.Count)
            {
                Debug.LogError($"Cannot select scheme {idx}, as it's outside the bounds of the currently loaded schemes");
                return;
            }

            _currentSchemeHandle = idx;
        }

        private void initInputDevice(InputDevice device)
        {
            Debug.Log($"Setting initial input values for {device.Name}");
            device.LeftStick.LowerDeadZone = 0.2f;
        }
    }
}
