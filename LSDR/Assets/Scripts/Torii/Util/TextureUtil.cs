using UnityEngine;

namespace Torii.Util
{
    public static class TextureUtil
    {
        public static Texture2D CreateColor(Color c)
        {
            Texture2D tex = new Texture2D(width: 1, height: 1);
            tex.SetPixel(x: 0, y: 0, c);
            tex.Apply();
            return tex;
        }
    }
}
