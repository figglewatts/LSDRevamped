using libLSD.Formats;
using LSDR.IO;
using UnityEngine;

namespace LSDR.Visual
{
    public class TIXTexture2D
    {
        public TIX Tix { get; }
        public Texture2D Texture { get; }

        public TIXTexture2D(TIX tix)
        {
            Tix = tix;
            Texture = LibLSDUnity.GetTextureFromTIX(tix);
        }

        public static implicit operator Texture2D(TIXTexture2D tixTex) { return tixTex.Texture; }
    }
}
