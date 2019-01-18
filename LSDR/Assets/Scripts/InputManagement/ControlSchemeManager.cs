using System;
using System.Collections.Generic;
using System.IO;
using Game;
using Torii.Serialization;
using Util;
using UnityEngine;

namespace InputManagement
{
	public static class ControlSchemeManager
    {
        public static List<ControlScheme> Schemes;

        public static ControlScheme Current => Schemes[_currentControlSchemeIndex];

        private static readonly ToriiSerializer _serializer = new ToriiSerializer();

        private static int _currentControlSchemeIndex;

        public static int CurrentSchemeIndex => _currentControlSchemeIndex;

        private static string PathToControlSchemes =>
            IOUtil.PathCombine(Application.persistentDataPath, "input-schemes");

        private static List<ControlScheme> deserializeControlSchemes(string path)
        {
            List<ControlScheme> schemes = new List<ControlScheme>();

            if (!Directory.Exists(PathToControlSchemes))
            {
                Directory.CreateDirectory(PathToControlSchemes);
            }

            foreach (string file in Directory.GetFiles(path, "*.dat"))
            {
                schemes.Add(_serializer.Deserialize<ControlScheme>(file));
            }

            return schemes;
        }

        public static void SerializeControlSchemes(List<ControlScheme> schemes)
        {
            foreach (var scheme in schemes)
            {
                _serializer.Serialize(scheme, IOUtil.PathCombine(PathToControlSchemes, scheme.Name + ".dat"));
            }
        }

        public static void ReloadSchemes() { Schemes = deserializeControlSchemes(PathToControlSchemes); }

        public static void UseScheme(int idx)
        {
            if (idx > Schemes.Count)
            {
                Debug.LogError(
                    $"Cannot select control scheme with index {idx} - exceeds scheme count: {Schemes.Count}!");
                return;
            }
            Debug.Log($"Using control scheme {idx}: {Schemes[idx].Name}");
            _currentControlSchemeIndex = idx;
        }

        public static void Initialize()
        {
            Schemes = deserializeControlSchemes(PathToControlSchemes);

            // if no schemes are present create and serialise the default scheme
            if (Schemes.Count <= 0)
            {
                Debug.Log("No control schemes found - creating new ones!");
                Schemes.Add(new ControlScheme(ControlActions.CreateDefaultTank(), "Classic", false));
                Schemes.Add(new ControlScheme(ControlActions.CreateDefaultFps(), "Revamped", true, 5F));
                SerializeControlSchemes(Schemes);
            }
        }
    }
}
