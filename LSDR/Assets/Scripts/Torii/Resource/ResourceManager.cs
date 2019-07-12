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
    public static class ResourceManager
    {
        private static readonly Dictionary<string, GenericResource> _resources;
        private static readonly Dictionary<Type, IResourceHandler> _handlers;
        private static readonly Dictionary<Type, ITextAssetHandler> _textAssetProcessors;
        private static readonly ResourceLifespans _lifespans;

        private static readonly string lifespansDataFileName = "resourcelifespans.json";

        static ResourceManager()
        {
            _resources = new Dictionary<string, GenericResource>();
            _handlers = new Dictionary<Type, IResourceHandler>();
            _textAssetProcessors = new Dictionary<Type, ITextAssetHandler>();
            _lifespans = new ResourceLifespans();
        }

        public static void Initialize()
        {
        }

        public static void ClearLifespan(string span)
        {
            int spanID = _lifespans[span];

            List<string> toRemove = new List<string>();
            foreach (var res in _resources)
            {
                if (res.Value.Lifespan == spanID)
                {
                    toRemove.Add(res.Key);
                    res.Value.OnExpire?.Invoke();
                }
            }

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

        public static void ClearUnusedUnityAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        public static T Load<T>(string path)
        {
            return Load<T>(path, _lifespans["global"]);
        }

        public static T Load<T>(string path, string span)
        {
            return Load<T>(path, _lifespans[span]);
        }

        public static T Load<T>(string path, int span)
        {
            Resource<T> res;
            if (checkCache(path, out res)) return res.Data;

            path = path.Trim();

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

            _handlers[typeof(T)].Load(path, span);

            return ((Resource<T>)_resources[path]).Data;
        }

        public static T UnityLoad<T>(string path) where T : class
        {
            return UnityLoad<T>(path, _lifespans["global"]);
        }

        public static T UnityLoad<T>(string path, string span) where T : class
        {
            return UnityLoad<T>(path, _lifespans[span]);
        }

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

        public static void RegisterResource(string path, GenericResource r)
        {
            _resources[path] = r;
        }

        public static void RegisterHandler(IResourceHandler handler)
        {
            _handlers[handler.HandlerType] = handler;
        }

        public static void RegisterTextAssetProcessor(ITextAssetHandler handler)
        {
            _textAssetProcessors[handler.HandlerType] = handler;
        }

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