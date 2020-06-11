using Torii.Util;
using UnityEngine;

namespace LSDR.Visual
{
    public struct TextureSetOptions
    {
        public string APath;
        public string BPath;
        public string CPath;
        public string DPath;
        public Shader ClassicShader;
        public Shader RevampedShader;

        public TextureSetOptions(string a, string b, string c, string d, Shader classic, Shader revamped)
        {
            APath = a;
            BPath = b;
            CPath = c;
            DPath = d;
            ClassicShader = classic;
            RevampedShader = revamped;
        }

        public static TextureSetOptions GetFromLBDPath(string lbdPath, Shader classic, Shader revamped)
        {
            var tixRoot = PathUtil.Combine(lbdPath, "TEX");
            return new TextureSetOptions($"{tixRoot}A.TIX", $"{tixRoot}B.TIX", $"{tixRoot}C.TIX", $"{tixRoot}D.TIX",
                classic, revamped);
        }
    }
}
