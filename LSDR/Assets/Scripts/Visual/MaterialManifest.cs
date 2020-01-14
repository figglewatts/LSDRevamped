using Newtonsoft.Json;
using System.Collections.Generic;
using Torii.Resource;
using UnityEngine;
using LSDR.Util;

namespace LSDR.Visual
{
    /// <summary>
    /// A MaterialManifest is a JSON object that defines how to generate a Material.
    /// // TODO: obsolete
    /// </summary>
    [JsonObject]
    public class MaterialManifest
    {
        /// <summary>
        /// The name of this Material.
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }
        
        /// <summary>
        /// The Shader that this Material should use.
        /// </summary>
        public string Shader { get; set; }
        
        /// <summary>
        /// The tag of this Material, used for modifying Material properties in batch.
        /// </summary>
        public string Tag { get; set; }
        
        /// <summary>
        /// The list of texture names for this Material.
        /// </summary>
        [JsonProperty]
        public List<string> Textures { get; private set; }

        // the chars indicating texture number in a filename
        [JsonIgnore]
        private readonly char[] _texNumberChars = new[]
        {
            'A',
            'B',
            'C',
            'D'
        };

        // the cached ID of the _MainTex property
        [JsonIgnore]
        private static readonly int _mainTex = UnityEngine.Shader.PropertyToID("_MainTex");

        // the cached material
        private Material _generatedMat;

        /// <summary>
        /// Access the Material that has been generated from this manifest.
        /// </summary>
        public Material Generated => GenerateMaterial();

        /// <summary>
        /// Invalidate the cached generated material, so we can regenerate it with new parameters.
        /// </summary>
        public void InvalidateMaterial() { _generatedMat = null; }

        /// <summary>
        /// Generate the Material from this manifest.
        /// </summary>
        /// <returns>The generated Material.</returns>
        public Material GenerateMaterial()
        {
            // check if we've already generated this material before
            if (_generatedMat) return _generatedMat;
            
            // try and find the shader
            Shader shader = UnityEngine.Shader.Find(Shader);
            if (shader == null)
            {
                Debug.LogError($"Could not generate material '{Name}' - could not find shader: {Shader}");
                return null;
            }

            // make sure there aren't too many textures
            if (Textures.Count > 4)
            {
                Debug.LogError(
                    $"Could not generate material '{Name}' - Materials with more than 4 textures are not supported.");
                return null;
            }

            // create the material
            Material mat = new Material(UnityEngine.Shader.Find(Shader));
            if (Textures.Count == 1)
            {
                // if there's only one texture, try to load it
                string texPath = IOUtil.PathCombine(Application.streamingAssetsPath, Textures[0]);
                Texture2D tex = ResourceManager.Load<Texture2D>(texPath);
                mat.SetTexture(_mainTex, tex);
            }
            else if (Textures.Count > 1)
            {
                // otherwise, try to load all of the textures
                for (int i = 0; i < Textures.Count; i++)
                {
                    string texPath = IOUtil.PathCombine(Application.streamingAssetsPath, Textures[i]);
                    Texture2D tex = ResourceManager.Load<Texture2D>(texPath);
                    mat.SetTexture("_MainTex" + _texNumberChars[i], tex);
                }
            }

            // assign to the cached generated material, so we can get it easily later
            _generatedMat = mat;

            return mat;
        }
    }
}