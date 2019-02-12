using System;
using Newtonsoft.Json;
using Torii.Resource;
using UnityEngine;
using Visual;

namespace ResourceHandlers
{
    public class MaterialHandler : IResourceHandler
    {
        public Type HandlerType => typeof(MaterialManifest);

        public void Load(string path, int span)
        {
            MaterialManifest mf = JsonConvert.DeserializeObject<MaterialManifest>(path);
            
            MaterialRegistry.Register(mf);
            
            Resource<MaterialManifest> resource = new Resource<MaterialManifest>(mf, span);
            ResourceManager.RegisterResource(path, resource);

            resource.OnExpire += () => MaterialRegistry.Deregister(mf);
        }
    }
}
