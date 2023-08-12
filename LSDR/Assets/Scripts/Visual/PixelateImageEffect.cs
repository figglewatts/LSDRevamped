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
        private Camera _main;
        private int _width;

        protected void Start()
        {
            _main = Camera.main;
        }

        protected void Update()
        {
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
            Graphics.Blit(buffer, dest);
            RenderTexture.ReleaseTemporary(buffer);
        }
    }
}
