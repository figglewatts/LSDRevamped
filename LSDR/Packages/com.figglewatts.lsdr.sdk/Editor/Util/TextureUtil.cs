using UnityEngine;

namespace LSDR.SDK.Editor.Util
{
    public static class TextureUtil
    {
        public static Texture2D CreateColor(Color c)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, c);
            tex.Apply();
            return tex;
        }
    }
}
