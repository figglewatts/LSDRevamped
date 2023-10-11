using System;
using System.Collections.Generic;
using LSDR.SDK.Util;
using UnityEngine;
using UnityEngine.Rendering;

namespace LSDR.SDK.Visual
{
    public class TextureSetter : MonoBehaviour
    {
        protected static TextureSetter _instance;

        protected static readonly int _textureSetPropertyID = Shader.PropertyToID("_TextureSet");

        [NonSerialized] protected readonly List<Material> _textureSetMaterials = new List<Material>();
        protected Shader _classicAlphaShader;
        protected Shader _classicShader;
        protected Shader _classicWaterShader;

        protected bool _lastShaderSet;
        protected Shader _revampedAlphaShader;
        protected Shader _revampedShader;
        protected Shader _revampedWaterShader;

        protected TextureSet _textureSet;

        public static TextureSetter Instance
        {
            get
            {
                // if it was already created, return it
                if (_instance) return _instance;

                // if the object exists but we don't know about it, get it and return it
                _instance = FindObjectOfType<TextureSetter>();
                if (_instance) return _instance;

                // if the object doesn't exist, create it
                _instance = new GameObject("TextureSetter", typeof(TextureSetter)).GetComponent<TextureSetter>();
                DontDestroyOnLoad(_instance);
                return _instance;
            }
        }

        public TextureSet TextureSet
        {
            get => _textureSet;
            set
            {
                _textureSet = value;
                Shader.SetGlobalInt(_textureSetPropertyID, (int)value);
            }
        }

        public void Awake()
        {
            _classicShader = Shader.Find("LSDR/ClassicDiffuseSet");
            _classicAlphaShader = Shader.Find("LSDR/ClassicDiffuseSetAlphaBlend");
            _revampedShader = Shader.Find("LSDR/RevampedDiffuseSet");
            _revampedAlphaShader = Shader.Find("LSDR/RevampedDiffuseSetAlphaBlend");
            _classicWaterShader = Shader.Find("LSDR/ClassicWater");
            _revampedWaterShader = Shader.Find("LSDR/RevampedWater");
        }

        public void SetShader(Material mat) { SetShader(mat, _lastShaderSet); }

        public void SetShader(Material mat, bool classic)
        {
            if (mat.name.ToLowerInvariant().Contains("water"))
            {
                mat.shader = classic ? _classicWaterShader : _revampedWaterShader;
                return;
            }

            ShaderTagId queue = mat.shader.FindPassTagValue(passIndex: 0, new ShaderTagId("Queue"));
            if (queue.name.Equals("Transparent"))
                mat.shader = classic ? _classicAlphaShader : _revampedAlphaShader;
            else
                mat.shader = classic ? _classicShader : _revampedShader;
        }

        public void SetAllShaders(bool classic)
        {
            _lastShaderSet = classic;
            foreach (Material registeredMat in _textureSetMaterials) SetShader(registeredMat, classic);
        }

        public void RegisterMaterial(Material mat)
        {
            if (!isMaterialValid(mat)) return;
            _textureSetMaterials.Add(mat);
            SetShader(mat);
        }

        public void DeregisterMaterial(Material mat) { _textureSetMaterials.Remove(mat); }

        public void DeregisterAllMaterials() { _textureSetMaterials.Clear(); }

        public void SetRandomTextureSetFromDayNumber(int dayNumber)
        {
            // 4 texture sets, add a new one into the mix every 10 days -- hence the mod 41 here
            // (41 instead of 40, as day isn't zero indexed -- it begins at 1!)
            int dayNumWithinBounds = dayNumber % 41;

            if (dayNumWithinBounds <= 10)
            {
                // just normal
                TextureSet = TextureSet.Normal;
            }
            else if (dayNumWithinBounds <= 20)
            {
                // add kanji in
                TextureSet = RandUtil.From(TextureSet.Normal, TextureSet.Kanji);
            }
            else if (dayNumWithinBounds <= 30)
            {
                // add downer in
                TextureSet = RandUtil.From(TextureSet.Normal, TextureSet.Kanji, TextureSet.Downer);
            }
            else if (dayNumWithinBounds <= 40)
            {
                // full choice!
                TextureSet = RandUtil.RandomEnum<TextureSet>();
            }
            else
            {
                // shouldn't get here due to the mod 41
                TextureSet = TextureSet.Normal;
            }

            // TODO: randomly introduce the glitch texture set
        }

        protected bool isMaterialValid(Material mat)
        {
            var matShader = mat.shader;
            if (matShader == _classicShader) return true;
            if (matShader == _revampedShader) return true;
            if (matShader == _classicAlphaShader) return true;
            if (matShader == _revampedAlphaShader) return true;
            if (matShader == _classicWaterShader) return true;
            if (matShader == _revampedWaterShader) return true;
            return false;
        }
    }
}
