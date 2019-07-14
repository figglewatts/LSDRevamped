// PixelBoy script by @WTFMIG
// Slight modifications by Figglewatts

using UnityEngine;

namespace Visual
{
    /// <summary>
    /// Image effect for pixellating a camera.
    /// </summary>
    [ExecuteInEditMode]
    public class PixelateImageEffect : MonoBehaviour
    {
        public int Height = 64;
        private int _width;
        private Camera _main;

        protected void Start()
        {
            _main = Camera.main;
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                return;
            }
        }

        protected void Update()
        {
            float ratio = _main.pixelWidth / (float)_main.pixelHeight;
            _width = Mathf.RoundToInt(Height * ratio);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            src.filterMode = FilterMode.Point;
            RenderTexture buffer = RenderTexture.GetTemporary(_width, Height, -1);
            buffer.filterMode = FilterMode.Point;
            UnityEngine.Graphics.Blit(src, buffer);
            UnityEngine.Graphics.Blit(buffer, dest);
            RenderTexture.ReleaseTemporary(buffer);
        }
    }
}
