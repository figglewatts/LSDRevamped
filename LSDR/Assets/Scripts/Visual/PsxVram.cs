using LSDR.IO;
using libLSD.Formats;
using Torii.Resource;
using UnityEngine;
using LSDR.Util;

namespace LSDR.Visual
{
    /// <summary>
    /// Virtual PSX VRAM. A TIX file can be loaded into it to populate it.
    /// Renderers that want to use this need to set their materials to the ones in here.
    /// </summary>
    public static class PsxVram
    {
        // as it was in PSX datasheets
        public const int VRAM_WIDTH = 2056;
        public const int VRAM_HEIGHT = 512;

        /// <summary>
        /// The material for non alpha-blended polygons.
        /// </summary>
        public static Material VramMaterial => VramMaterialManifest.GenerateMaterial();
        public static MaterialManifest VramMaterialManifest;
        
        /// <summary>
        /// The material for alpha-blended polygons.
        /// </summary>
        public static Material VramAlphaBlendMaterial => VramAlphaBlendMaterialManifest.GenerateMaterial();
        public static MaterialManifest VramAlphaBlendMaterialManifest;
        public static Texture VramTexture;
        private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

        public static Material[] Materials => new[] {VramMaterial, VramAlphaBlendMaterial};

        private static readonly string _vramMaterialPath =
            IOUtil.PathCombine(Application.streamingAssetsPath, "materials", "psx-vram-diffuse.json");

        private static readonly string _vramAlphaMaterialPath =
            IOUtil.PathCombine(Application.streamingAssetsPath, "materials", "psx-vram-alpha.json");

        /// <summary>
        /// Initialize the virtual PSX VRAM. This needs to be called when the game starts.
        /// </summary>
        public static void Initialize()
        {
            VramMaterialManifest = ResourceManager.Load<MaterialManifest>(_vramMaterialPath);
            VramAlphaBlendMaterialManifest = ResourceManager.Load<MaterialManifest>(_vramAlphaMaterialPath);
        }

        /// <summary>
        /// Load a TIX into the virtual VRAM. 'Paints' textures within the TIX file to the virtual VRAM.
        /// </summary>
        /// <param name="tix">The TIX file to load into VRAM.</param>
        public static void LoadVramTix(TIX tix)
        {
            VramTexture = LibLSDUnity.GetTextureFromTIX(tix);
            VramMaterial.SetTexture(_mainTex, VramTexture);
            VramAlphaBlendMaterial.SetTexture(_mainTex, VramTexture);
        }
    }
}
