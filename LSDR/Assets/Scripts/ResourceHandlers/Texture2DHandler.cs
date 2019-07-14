using System;
using System.IO;
using Torii.Resource;
using UnityEngine;

namespace ResourceHandlers
{
    /// <summary>
    /// Load a PNG image from disk.
    /// </summary>
    public class Texture2DHandler : IResourceHandler
    {
        public Type HandlerType => typeof(Texture2D);

        public void Load(string path, int span)
        {
            string fullFilePath = path;
            string extension = Path.GetExtension(fullFilePath);
            
            // make sure the extension is correct
            if (string.IsNullOrEmpty(extension))
            {
                fullFilePath += ".png";
            }
            else if (!extension.Equals(".png"))
            {
                Debug.LogError($"Could not load texture! File extension {extension} not supported.");
                return;
            }

            byte[] fileData = File.ReadAllBytes(fullFilePath);
            
            // (2, 2) is temporary power of 2 size; next line will resize automatically
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false); 
            tex.LoadImage(fileData);
            tex.filterMode = FilterMode.Point;
            tex.mipMapBias = 0F;
    
            Resource<Texture2D> resource = new Resource<Texture2D>(tex, span);
            ResourceManager.RegisterResource(path, resource);
        }
    }
}
