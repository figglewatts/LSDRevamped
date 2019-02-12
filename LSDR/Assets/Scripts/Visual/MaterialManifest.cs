using Newtonsoft.Json;
using System.Collections.Generic;
using Torii.Resource;
using UnityEngine;
using Util;

namespace Visual
{
    [JsonObject]
    public class MaterialManifest
    {
        public string MaterialName { get; }
        public string Shader { get; set; }
        public string Tag { get; set; }
        public List<string> Textures { get; }

        [JsonIgnore]
        private readonly char[] _texNumberChars = new[]
        {
            'A',
            'B',
            'C',
            'D'
        };

        [JsonIgnore]
        private static readonly int _mainTex = UnityEngine.Shader.PropertyToID("_MainTex");

        private Material _generatedMat;

        public Material Generated => GenerateMaterial();

        public Material GenerateMaterial()
        {
            if (_generatedMat) return _generatedMat;
            else
            {
                Shader shader = UnityEngine.Shader.Find(Shader);
                if (shader == null)
                {
                    Debug.LogError($"Could not generate material '{MaterialName}' - could not find shader: {Shader}");
                    return null;
                }

                if (Textures.Count > 4)
                {
                    Debug.LogError(
                        $"Could not generate material '{MaterialName}' - Materials with more than 4 textures are not supported.");
                    return null;
                }

                Material mat = new Material(UnityEngine.Shader.Find(Shader));

                if (Textures.Count == 1)
                {
                    string texPath = IOUtil.PathCombine(Application.streamingAssetsPath, Textures[0]);
                    Texture2D tex = ResourceManager.Load<Texture2D>(texPath);
                    mat.SetTexture(_mainTex, tex);
                }
                else if (Textures.Count > 1)
                {
                    for (int i = 0; i < Textures.Count; i++)
                    {
                        string texPath = IOUtil.PathCombine(Application.streamingAssetsPath, Textures[i]);
                        Texture2D tex = ResourceManager.Load<Texture2D>(texPath);
                        mat.SetTexture("_MainTex" + _texNumberChars[i], tex);
                    }
                }

                return mat;
            }
        }
    }
}