using System;
using System.IO;
using libLSD.Formats;
using Torii.Resource;

namespace LSDR.IO.ResourceHandlers
{
    /// <summary>
    /// A ResourceHandler to handle loading LBD files.
    /// </summary>
    public class LBDHandler : IResourceHandler
    {
        public Type HandlerType => typeof(LBD);

        /// <summary>
        /// Load an LBD file from disk.
        /// </summary>
        /// <param name="path">The path to the LBD file.</param>
        /// <param name="span">The span ID for how long this resource should be cached.</param>
        public void Load(string path, int span)
        {
            LBD lbd;
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                lbd = new LBD(br);
            }
            
            Resource<LBD> resource = new Resource<LBD>(lbd, span);
            ResourceManager.RegisterResource(path, resource);
        }
    }
}
