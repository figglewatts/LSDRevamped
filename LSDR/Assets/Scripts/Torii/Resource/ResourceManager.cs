using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using Torii.Exceptions;
using Torii.UI;
using Torii.Util;
using UnityEngine;

namespace Torii.Resource
{
    /// <summary>
    /// ResourceManager is used to load/cache resources at runtime.
    /// </summary>
    public static class ResourceManager
    {
        // the cache of loaded resources, indexed by path they were loaded from
        private static readonly Dictionary<string, GenericResource> _resources;
        
        // the resource handlers to use by Type
        private static readonly Dictionary<Type, IResourceHandler> _handlers;
        
        // special handling for Unity text assets
        private static readonly Dictionary<Type, ITextAssetHandler> _textAssetProcessors;
        
        // the loaded ResourceLifespans
        private static readonly ResourceLifespans _lifespans;

        // the file to load resource lifespans from
        private static readonly string lifespansDataFileName = "resourcelifespans.json";

        /// <summary>
        /// Statically initialize the ResourceManager.
        /// </summary>
        static ResourceManager()
        {
            _resources = new Dictionary<string, GenericResource>();
            _handlers = new Dictionary<Type, IResourceHandler>();
            _textAssetProcessors = new Dictionary<Type, ITextAssetHandler>();
            _lifespans = new ResourceLifespans();
        }

        /// <summary>
        /// Remove all resources with the given lifespan from the cache.
        /// </summary>
        /// <param name="span">The lifespan, for example "level".</param>
        /// <exception cref="ArgumentException">If the lifespan didn't exist.</exception>
        public static void ClearLifespan(string span)
        {
            // get the ID of this span
            int spanID = _lifespans[span];

            // get the list of resources to remove
            List<string> toRemove = new List<string>();
            foreach (var res in _resources)
            {
                if (res.Value.Lifespan == spanID)
                {
                    toRemove.Add(res.Key);
                    res.Value.OnExpire?.Invoke();
                }
            }

            // remove the resources
            foreach (string key in toRemove)
            {
                switch (_resources[key].ResourceType)
                {
                    case ResourceType.Streamed:
                        _resources.Remove(key);
                        break;
                    case ResourceType.Unity:
                        Resources.UnloadAsset((UnityEngine.Object)_resources[key].GetData());
                        _resources.Remove(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Unity assets are handled differently. This unloads all unused Unity resources.
        /// </summary>
        public static void ClearUnusedUnityAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Load a resource with type T from a location on disk. Loads it for global lifespan.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The loaded resource.</returns>
        public static T Load<T>(string path)
        {
            return Load<T>(path, _lifespans["global"]);
        }

        /// <summary>
        /// Load a resource with type T from a location on disk, and give it a lifespan.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="span">The lifespan of the resource.</param>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The loaded resource.</returns>
        /// <exception cref="ArgumentException">If the lifespan didn't exist.</exception>
        public static T Load<T>(string path, string span)
        {
            return Load<T>(path, _lifespans[span]);
        }

        /// <summary>
        /// Load a resource with type T from a location on disk, and give it a lifespan with an ID.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="span">The lifespan of the resource.</param>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The loaded resource.</returns>
        /// <exception cref="ArgumentException">If the path is empty, or the handler for type wasn't found.</exception>
        /// <exception cref="FileNotFoundException">If the file was not found.</exception>
        public static T Load<T>(string path, int span)
        {
            Resource<T> res;
            
            // check the cache first, as this would be cheaper
            if (checkCache(path, out res)) return res.Data;

            path = path.Trim();

            // do a bunch of checks to see if this resource can actually be loaded
            if (path.Equals(string.Empty))
            {
                throw new ArgumentException("Could not load resource: path argument cannot be empty", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not load resource: File '" + path + "' not found", path);
            }

            if (!_handlers.ContainsKey(typeof(T)))
            {
                throw new ArgumentException("Could not load resource: No handler found for type " + typeof(T));
            }

            // load the resource
            _handlers[typeof(T)].Load(path, span);

            return ((Resource<T>)_resources[path]).Data;
        }

        /// <summary>
        /// Load a resource from Unity's Resources.
        /// </summary>
        /// <param name="path">The path to the resource (in Unity's assets).</param>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The loaded resource.</returns>
        public static T UnityLoad<T>(string path) where T : class
        {
            return UnityLoad<T>(path, _lifespans["global"]);
        }

        /// <summary>
        /// Load a resource from Unity's resource.
        /// </summary>
        /// <param name="path">The path to the resource (in Unity's assets).</param>
        /// <param name="span">The lifespan to give this resource.</param>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The loaded resource.</returns>
        /// <exception cref="ArgumentException">If the given lifespan was invalid.</exception>
        public static T UnityLoad<T>(string path, string span) where T : class
        {
            return UnityLoad<T>(path, _lifespans[span]);
        }

        /// <summary>
        /// Load a resource from Unity's resource.
        /// </summary>
        /// <param name="path">The path to the resource (in Unity's assets).</param>
        /// <param name="span">The lifespan to give this resource.</param>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The loaded resource.</returns>
        /// <exception cref="ToriiResourceLoadException">If there was some error loading the resource.</exception>
        public static T UnityLoad<T>(string path, int span) where T : class
        {
            // prepend "Resources" to the start of the path to prevent edge cases
            // where the same name file in StreamingAssets could conflict
            string resourcePath = PathUtil.Combine("Resources", path);

            Resource<T> res;
            if (checkCache(resourcePath, out res)) return res.Data;

            // check if the asset can be processed as text
            if (_textAssetProcessors.ContainsKey(typeof(T)))
            {
                ITextAssetHandler handler = _textAssetProcessors[typeof(T)];
                TextAsset textAsset = Resources.Load<TextAsset>(path);

                // check to see if we loaded successfully
                if (textAsset == null)
                {
                    throw new ToriiResourceLoadException("Unable to load resource: '" + path + "'", typeof(T));
                }
                
                res = new Resource<T>(span)
                {
                    Data = (T)handler.Process(textAsset)
                };

                // we don't need the text asset anymore
                Resources.UnloadAsset(textAsset);
            }
            else
            {
                // otherwise just load it using Unity's resource handling
                res = new Resource<T>(span, ResourceType.Unity)
                {
                    Data = Resources.Load(path, typeof(T)) as T
                };
            }

            if (res.Data == null)
            {
                throw new ToriiResourceLoadException("Unable to load resource: '" + path + "'", typeof(T));
            }

            _resources[resourcePath] = res;

            return res.Data;
        }

        /// <summary>
        /// Used to register a loaded resource into the cache. Only call this from within custom resource handlers.
        /// </summary>
        /// <param name="path">The path of the resource you're loading.</param>
        /// <param name="r">The instance of the resource loaded.</param>
        public static void RegisterResource(string path, GenericResource r)
        {
            _resources[path] = r;
        }

        /// <summary>
        /// Register a new resource handler.
        /// </summary>
        /// <param name="handler">An instance of the handler.</param>
        public static void RegisterHandler(IResourceHandler handler)
        {
            _handlers[handler.HandlerType] = handler;
        }

        /// <summary>
        /// Register a new text asset processor.
        /// </summary>
        /// <param name="handler">An instance of the text asset handler.</param>
        public static void RegisterTextAssetProcessor(ITextAssetHandler handler)
        {
            _textAssetProcessors[handler.HandlerType] = handler;
        }

        // check the cache to see if a resource exists
        private static bool checkCache<T>(string path, out T data) where T : class
        {
            if (_resources.ContainsKey(path))
            {
                data = _resources[path] as T;
                return true;
            }
            data = null;
            return false;
        }
    }
}