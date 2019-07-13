using System;
using System.Collections.Generic;
using System.IO;
using Game;
using Torii.Serialization;
using Util;
using UnityEngine;

namespace InputManagement
{
	/// <summary>
	/// ControlSchemeManager is used to manage all of the user's control schemes.
	/// </summary>
    public static class ControlSchemeManager
    {
        /// <summary>
        /// The list of loaded control schemes.
        /// </summary>
        public static List<ControlScheme> Schemes;

        /// <summary>
        /// A handle to the current control scheme.
        /// </summary>
        public static ControlScheme Current => Schemes[_currentControlSchemeIndex];

        // serializer used for saving/loading control schemes
        private static readonly ToriiSerializer _serializer = new ToriiSerializer();

        // private member of CurrentSchemeIndex
        private static int _currentControlSchemeIndex;

        /// <summary>
        /// The index (into array of loaded schemes) of the currently loaded control scheme.
        /// </summary>
        public static int CurrentSchemeIndex => _currentControlSchemeIndex;

        // the path of the directory to save control schemes into
        private static string PathToControlSchemes =>
            IOUtil.PathCombine(Application.persistentDataPath, "input-schemes");

        // deserialize control schemes from a given path
        private static List<ControlScheme> deserializeControlSchemes(string path)
        {
            List<ControlScheme> schemes = new List<ControlScheme>();

            // if the directory doesn't exist, create it
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // deserialize all of the control schemes in the path
            foreach (string file in Directory.GetFiles(path, "*.dat"))
            {
                schemes.Add(_serializer.Deserialize<ControlScheme>(file));
            }

            return schemes;
        }

        /// <summary>
        /// Serialize a given list of control schemes.
        /// </summary>
        /// <param name="schemes">The list of control schemes to serialize.</param>
        public static void SerializeControlSchemes(List<ControlScheme> schemes)
        {
            foreach (var scheme in schemes)
            {
                _serializer.Serialize(scheme, IOUtil.PathCombine(PathToControlSchemes, scheme.Name + ".dat"));
            }
        }

        /// <summary>
        /// Reload all of the control schemes from the directory.
        /// </summary>
        public static void ReloadSchemes() { Schemes = deserializeControlSchemes(PathToControlSchemes); }

        /// <summary>
        /// Switch to using the control scheme at given index in loaded schemes array.
        /// </summary>
        /// <param name="idx">The index of the scheme to use.</param>
        public static void UseScheme(int idx)
        {
            // check if this index is valid
            if (idx > Schemes.Count)
            {
                Debug.LogError(
                    $"Cannot select control scheme with index {idx} - exceeds scheme count: {Schemes.Count}!");
                return;
            }
            Debug.Log($"Using control scheme {idx}: {Schemes[idx].Name}");
            _currentControlSchemeIndex = idx;
        }

        /// <summary>
        /// Initialize the control scheme manager. Should be called on game start.
        /// </summary>
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
