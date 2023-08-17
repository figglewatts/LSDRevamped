using System.IO;
using LSDR.SDK;
using Torii.Util;

namespace LSDR.Util
{
    public static class LSDUtil
    {
        public static string GetTIXPathFromLBDPath(string lbdPath, TextureSet textureSet)
        {
            string lbdFolder = lbdPath;
            if (File.Exists(lbdPath))
            {
                lbdFolder = Path.GetDirectoryName(lbdPath);
            }

            switch (textureSet)
            {
                default:
                {
                    return PathUtil.Combine(lbdFolder, "TEXA.TIX");
                }
                case TextureSet.Kanji:
                {
                    return PathUtil.Combine(lbdFolder, "TEXB.TIX");
                }
                case TextureSet.Downer:
                {
                    return PathUtil.Combine(lbdFolder, "TEXC.TIX");
                }
                case TextureSet.Upper:
                {
                    return PathUtil.Combine(lbdFolder, "TEXD.TIX");
                }
            }
        }
    }
}
