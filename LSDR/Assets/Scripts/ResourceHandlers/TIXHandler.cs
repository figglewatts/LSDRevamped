using System;
using System.IO;
using libLSD.Formats;
using Torii.Resource;

namespace ResourceHandlers
{
    /// <summary>
    /// Load a TIX texture archive from disk.
    /// </summary>
    public class TIXHandler : IResourceHandler
    {
        public Type HandlerType => typeof(TIX);

        public void Load(string path, int span)
        {
            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                tix = new TIX(br);
            }
            
            Resource<TIX> resource = new Resource<TIX>(tix, span);
            ResourceManager.RegisterResource(path, resource);
        }
    }
}
