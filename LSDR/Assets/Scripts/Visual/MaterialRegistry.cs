using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Visual
{
    public static class MaterialRegistry
    {
        private static readonly Dictionary<string, List<MaterialManifest>> _registry;

        static MaterialRegistry()
        {
            _registry = new Dictionary<string, List<MaterialManifest>>();
        }

        public static void Register(MaterialManifest mat)
        {
            if (!_registry.ContainsKey(mat.Tag))
            {
                _registry[mat.Tag] = new List<MaterialManifest>();
            }
            _registry[mat.Tag].Add(mat);
        }

        public static void ChangeShader(string tag, string shader)
        {
            if (!_registry.ContainsKey(tag))
            {
                Debug.LogError($"Can't change shader - Material registry doesn't contain tag '{tag}'");
                return;
            }
            
            Shader s = Shader.Find(shader);
            if (s == null)
            {
                Debug.LogError($"Can't change shader - Could not find shader '{shader}'.");
                return;
            }
            
            foreach (var mat in _registry[tag])
            {
                if (mat == null) continue;
                mat.Generated.shader = s;
            }
        }

        public static void Deregister(MaterialManifest mat)
        {
            if (_registry.ContainsKey(mat.Tag))
            {
                _registry[mat.Tag].Remove(mat);
            }
        }
    }
}
