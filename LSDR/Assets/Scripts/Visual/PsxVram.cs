using IO;
using libLSD.Formats;
using Torii.Resource;
using UnityEngine;
using Util;

namespace Visual
{
    public static class PsxVram
    {
        public const int VRAM_WIDTH = 2056;
        public const int VRAM_HEIGHT = 512;

        public static Material VramMaterial;
        public static Material VramAlphaBlendMaterial;
        public static Texture VramTexture;
        private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

        public static Material[] Materials => new[] {VramMaterial, VramAlphaBlendMaterial};

        private static string _vramMaterialPath =
            IOUtil.PathCombine(Application.streamingAssetsPath, "materials", "psx-vram-diffuse.json");

        private static string _vramAlphaMaterialPath =
            IOUtil.PathCombine(Application.streamingAssetsPath, "materials", "psx-vram-alpha.json");

        public static void Initialize()
        {
            VramMaterial = ResourceManager.Load<Material>(_vramMaterialPath);
            VramAlphaBlendMaterial = ResourceManager.Load<Material>(_vramAlphaMaterialPath);
        }

        public static void LoadVramTix(TIX tix)
        {
            VramTexture = LibLSDUnity.GetTextureFromTIX(tix);
            VramMaterial.SetTexture(_mainTex, VramTexture);
            VramAlphaBlendMaterial.SetTexture(_mainTex, VramTexture);
            
        }
    }
}
