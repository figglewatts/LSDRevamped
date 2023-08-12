using libLSD.Formats;
using LSDR.SDK;
using UnityEngine;

namespace LSDR.Visual
{
    public class TIXTexture2D
    {
        public TIXTexture2D(TIX tix)
        {
            Tix = tix;
            Texture = LibLSDUnity.GetTextureFromTIX(tix);
        }

        public TIX Tix { get; }
        public Texture2D Texture { get; }

        public static implicit operator Texture2D(TIXTexture2D tixTex) { return tixTex.Texture; }
    }
}
