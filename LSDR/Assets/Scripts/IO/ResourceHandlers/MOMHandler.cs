using System;
using System.IO;
using libLSD.Formats;
using Torii.Resource;

namespace LSDR.IO.ResourceHandlers
{
    /// <summary>
    ///     A ResourceHandler to handle loading MOM files.
    /// </summary>
    public class MOMHandler : IResourceHandler
    {
        public Type HandlerType => typeof(MOM);

        /// <summary>
        ///     Load a MOM file from disk.
        /// </summary>
        /// <param name="path">The path to the MOM file.</param>
        /// <param name="span">The span ID for how long this resource should be cached.</param>
        public void Load(string path, int span)
        {
            MOM mom;
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                mom = new MOM(br);
            }

            var resource = new Resource<MOM>(mom, span);
            ResourceManager.RegisterResource(path, resource);
        }
    }
}
