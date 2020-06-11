using System;
using System.Collections.Generic;
using System.IO;
using LSDR.Entities.Dream;
using Torii.Resource;
using UnityEngine;

namespace LSDR.Visual
{
    [CreateAssetMenu(menuName = "System/TextureSetSystem")]
    public class TextureSetSystem : ScriptableObject
    {
        [NonSerialized] public TextureSet CurrentTextureSet;

        [NonSerialized] private readonly List<RegisteredTextureSetMaterial> _textureSetMaterials =
            new List<RegisteredTextureSetMaterial>();

        public void SetTextureSet(TextureSet set)
        {
            foreach (var registeredMat in _textureSetMaterials)
            {
                registeredMat.ApplySet(set);
            }

            CurrentTextureSet = set;
        }

        public void SetShader(bool classic)
        {
            foreach (var registeredMat in _textureSetMaterials)
            {
                registeredMat.SetShader(classic);
            }
        }

        public void RegisterMaterial(Material mat, TextureSetOptions opts)
        {
            var registered = new RegisteredTextureSetMaterial(mat, opts);
            _textureSetMaterials.Add(registered);
            registered.ApplySet(CurrentTextureSet);
        }

        public void DeregisterMaterial(Material mat) { _textureSetMaterials.RemoveAll(x => x.Material == mat); }

        public void DeregisterAllMaterials() { _textureSetMaterials.Clear(); }

        internal class RegisteredTextureSetMaterial
        {
            public Material Material { get; }
            public TextureSetOptions Options { get; }

            public RegisteredTextureSetMaterial(Material mat, TextureSetOptions opts)
            {
                Material = mat;
                Options = opts;
            }

            public void ApplySet(TextureSet set)
            {
                switch (set)
                {
                    default:
                        applyTexturePath(Options.APath);
                        break;
                    case TextureSet.Kanji:
                        applyTexturePath(Options.BPath);
                        break;
                    case TextureSet.Downer:
                        applyTexturePath(Options.CPath);
                        break;
                    case TextureSet.Upper:
                        applyTexturePath(Options.DPath);
                        break;
                }
            }

            public void SetShader(bool classic)
            {
                Material.shader = classic ? Options.ClassicShader : Options.RevampedShader;
            }

            private void applyTexturePath(string path)
            {
                if (!File.Exists(path))
                {
                    Debug.LogError($"Unable to apply texture from set, '{path}' did not exist.");
                    return;
                }

                var ext = Path.GetExtension(path).ToLowerInvariant();
                if (ext.Equals(".tix"))
                {
                    TIXTexture2D tex = ResourceManager.Load<TIXTexture2D>(path, "scene");
                    Material.mainTexture = tex;
                }
                else if (ext.Equals(".png"))
                {
                    Texture2D tex = ResourceManager.Load<Texture2D>(path, "scene");
                    Material.mainTexture = tex;
                }
                else
                {
                    Debug.LogError($"Unable to apply texture from set, file type '{ext}' not supported.");
                }
            }
        }
    }
}
