using System;
using System.IO;
using Newtonsoft.Json;
using Torii.Exceptions;
using Torii.Resource;
using UnityEngine;
using Visual;

namespace ResourceHandlers
{
    /// <summary>
    /// Load a Material manifest from disk.
    /// </summary>
    public class MaterialHandler : IResourceHandler
    {
        public Type HandlerType => typeof(MaterialManifest);

        private readonly JsonSerializer _serializer = new JsonSerializer();

        public void Load(string path, int span)
        {
            MaterialManifest mf = null;
            
            // load the JSON file
            using (StreamReader file = File.OpenText(path))
            {
                mf = _serializer.Deserialize<MaterialManifest>(new JsonTextReader(file));
            }
            
            // register the manifest
            MaterialRegistry.Register(mf);
            
            Resource<MaterialManifest> resource = new Resource<MaterialManifest>(mf, span);
            ResourceManager.RegisterResource(path, resource);

            resource.OnExpire += () => MaterialRegistry.Deregister(mf);
        }
    }
}
