// PixelBoy script by @WTFMIG
// Slight modifications by Figglewatts

using UnityEngine;

namespace LSDR.Visual
{
    /// <summary>
    ///     Image effect for pixellating a camera.
    /// </summary>
    [ExecuteInEditMode]
    public class PixelateImageEffect : MonoBehaviour
    {
        public int Height = 64;
        public bool Dithering = true;
        private Camera _main;
        private int _width;

        public Material ImageEffectMaterial;

        protected static readonly int _ditherPattern = Shader.PropertyToID("_DitherPattern");

        protected const string SHADER = "Jazz/PSX Dither";

        protected void Start()
        {
            _main = Camera.main;
        }

        protected void Update()
        {
            if (_main == null) return;

            float ratio = _main.pixelWidth / (float)_main.pixelHeight;
            _width = Mathf.RoundToInt(Height * ratio);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_width == 0) return;

            src.filterMode = FilterMode.Point;
            RenderTexture buffer = RenderTexture.GetTemporary(_width, Height, depthBuffer: -1);
            buffer.filterMode = FilterMode.Point;
            Graphics.Blit(src, buffer);
            if (Dithering)
            {
                Graphics.Blit(buffer, dest, ImageEffectMaterial);
            }
            else
            {
                Graphics.Blit(buffer, dest);
            }
            RenderTexture.ReleaseTemporary(buffer);
        }
    }
}
