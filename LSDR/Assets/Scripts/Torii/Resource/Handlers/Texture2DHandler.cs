using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Torii.Resource.Handlers
{
    public class Texture2DHandler : IResourceHandler
    {
        public Type HandlerType => typeof(Texture2D);

        public void Load(string path, int span)
        {
            byte[] fileData = File.ReadAllBytes(path);

            // size of 2,2 is arbitrary, it gets resized next line anyway
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            Resource<Texture2D> resource = new Resource<Texture2D>(tex, span);
            ResourceManager.RegisterResource(path, resource);
        }
    }
}