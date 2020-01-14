using System.Collections.Generic;
using UnityEngine;

namespace LSDR.Visual
{
    /// <summary>
    /// MaterialRegistry is used to keep track of which Materials are used in a scene, so that properties/shaders
    /// can be swapped out at runtime.
    /// TODO: obsolete
    /// </summary>
    public static class MaterialRegistry
    {
        private static readonly Dictionary<string, List<MaterialManifest>> _registry; // the registry

        /// <summary>
        /// Initialize the registry.
        /// </summary>
        static MaterialRegistry()
        {
            _registry = new Dictionary<string, List<MaterialManifest>>();
        }

        /// <summary>
        /// Register a MaterialManifest with the material registry.
        /// </summary>
        /// <param name="mat">The MaterialManifest to register.</param>
        public static void Register(MaterialManifest mat)
        {
            if (!_registry.ContainsKey(mat.Tag))
            {
                _registry[mat.Tag] = new List<MaterialManifest>();
            }
            _registry[mat.Tag].Add(mat);
        }

        /// <summary>
        /// For each manifest with given tag, swap the shader to the given shader.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="shader">The shader to swap to.</param>
        public static void ChangeShader(string tag, string shader)
        {
            // check that the tag exists
            if (!_registry.ContainsKey(tag))
            {
                Debug.LogError($"Can't change shader - Material registry doesn't contain tag '{tag}'");
                return;
            }
            
            // check that the shader exists
            Shader s = Shader.Find(shader);
            if (s == null)
            {
                Debug.LogError($"Can't change shader - Could not find shader '{shader}'.");
                return;
            }
            
            // swap the shader out
            foreach (var mat in _registry[tag])
            {
                if (mat == null) continue;
                mat.Generated.shader = s;
            }
        }

        /// <summary>
        /// Deregister a MaterialManifest.
        /// </summary>
        /// <param name="mat">The MaterialManifest to deregister.</param>
        public static void Deregister(MaterialManifest mat)
        {
            if (_registry.ContainsKey(mat.Tag))
            {
                _registry[mat.Tag].Remove(mat);
            }
        }
    }
}
