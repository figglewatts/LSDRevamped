using System;
using System.IO;
using libLSD.Formats;
using LSDR.Visual;
using Torii.Resource;

namespace LSDR.IO.ResourceHandlers
{
    /// <summary>
    /// Load a TIX texture archive from disk into a Texture2D.
    /// </summary>
    public class TIXTexture2DHandler : IResourceHandler
    {
        public Type HandlerType => typeof(TIXTexture2D);

        public void Load(string path, int span)
        {
            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                tix = new TIX(br);
            }

            TIXTexture2D tixTex = new TIXTexture2D(tix);
            Resource<TIXTexture2D> resource = new Resource<TIXTexture2D>(tixTex, span);
            ResourceManager.RegisterResource(path, resource);
        }
    }
}
