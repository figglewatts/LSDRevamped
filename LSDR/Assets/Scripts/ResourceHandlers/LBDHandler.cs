using System;
using System.IO;
using IO;
using libLSD.Formats;
using Torii.Resource;
using UnityEngine;

namespace ResourceHandlers
{
    public class LBDHandler : IResourceHandler
    {
        public Type HandlerType => typeof(LBD);

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
