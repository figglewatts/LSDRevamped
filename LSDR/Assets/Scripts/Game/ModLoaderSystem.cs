using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LSDR.SDK.Data;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName = "System/ModLoaderSystem")]
    public class ModLoaderSystem : ScriptableObject
    {
        public List<string> BuiltInMods;

        protected readonly List<LSDRevampedMod> _loadedMods = new List<LSDRevampedMod>();
        public List<LSDRevampedMod> Mods => _loadedMods;

        protected string _modsDirectory => Path.Combine(Application.streamingAssetsPath, "mods");

        public bool ModsAvailable => _loadedMods.Count > 0;

        public int ModsCount => _loadedMods.Count;

        public LSDRevampedMod GetMod(int modIdx)
        {
            if (modIdx >= _loadedMods.Count)
                modIdx = _loadedMods.Count - 1;
            else if (modIdx < 0) modIdx = 0;

            var mod = _loadedMods[modIdx];
            return mod;
        }

        public IEnumerator LoadModsAsync()
        {
            Debug.Log("Loading mods...");

            if (_loadedMods.Count > 0)
            {
                Debug.LogWarning("Cannot load mods twice!");
                yield break;
            }

            ensureDirectory();

            foreach (var builtInPath in BuiltInMods)
            {
                ResourceRequest req = Resources.LoadAsync<LSDRevampedMod>(builtInPath);
                yield return req;
                _loadedMods.Add((LSDRevampedMod)req.asset);
            }

            string[] modDirectories = Directory.GetDirectories(_modsDirectory);
            foreach (var modFolder in modDirectories)
            {
                var modFolderWithPlatform = Path.Combine(modFolder, getPlatformPathFragment());
                if (!Directory.Exists(modFolderWithPlatform))
                {
                    Debug.LogWarning(
                        $"Unable to load mod '{Path.GetDirectoryName(modFolder)}', no mod folder for platform {getPlatformPathFragment()}");
                    continue;
                }

                string[] modFiles =
                    Directory.GetFiles(modFolderWithPlatform, "*.lsdrmod", SearchOption.AllDirectories);
                if (modFiles.Length <= 0)
                {
                    Debug.LogWarning(
                        $"Unable to load mod '{Path.GetDirectoryName(modFolder)}', no mod for platform {getPlatformPathFragment()}");
                    continue;
                }

                foreach (string modFile in modFiles)
                {
                    var bundleLoadRequest = AssetBundle.LoadFromFileAsync(modFile);
                    yield return bundleLoadRequest;
                    if (bundleLoadRequest == null)
                    {
                        Debug.LogWarning($"Unable to load mod '{modFile}', error loading asset bundle");
                        continue;
                    }

                    var modLoadRequest = bundleLoadRequest.assetBundle.LoadAllAssetsAsync<LSDRevampedMod>();
                    yield return modLoadRequest;

                    if (modLoadRequest.allAssets.Length > 1)
                    {
                        Debug.LogWarning($"Unable to load mod '{modFile}', multiple LSDRevampedMods in bundle");
                        continue;
                    }

                    if (modLoadRequest.allAssets.Length == 0)
                    {
                        Debug.LogWarning($"Unable to load mod '{modFile}', no LSDRevampedMod in bundle");
                        continue;
                    }

                    LSDRevampedMod mod = (LSDRevampedMod)modLoadRequest.allAssets[0];
                    mod.SetSourceBundle(bundleLoadRequest.assetBundle);
                    _loadedMods.Add(mod);
                }
            }

            yield return null;
        }

        public void LoadMods()
        {
            Debug.Log("Loading mods...");

            if (_loadedMods.Count > 0)
            {
                Debug.LogWarning("Cannot load mods twice!");
                return;
            }

            ensureDirectory();

            foreach (var builtInPath in BuiltInMods)
            {
                var mod = Resources.Load<LSDRevampedMod>(builtInPath);
                _loadedMods.Add(mod);
            }

            string[] modDirectories = Directory.GetDirectories(_modsDirectory);
            foreach (var modFolder in modDirectories)
            {
                var modFolderWithPlatform = Path.Combine(modFolder, getPlatformPathFragment());
                if (!Directory.Exists(modFolderWithPlatform))
                {
                    Debug.LogWarning(
                        $"Unable to load mod '{Path.GetDirectoryName(modFolder)}', no mod folder for platform {getPlatformPathFragment()}");
                    continue;
                }

                string[] modFiles =
                    Directory.GetFiles(modFolderWithPlatform, "*.lsdrmod", SearchOption.AllDirectories);
                if (modFiles.Length <= 0)
                {
                    Debug.LogWarning(
                        $"Unable to load mod '{Path.GetDirectoryName(modFolder)}', no mod for platform {getPlatformPathFragment()}");
                    continue;
                }

                foreach (string modFile in modFiles)
                {
                    AssetBundle loadedBundle = AssetBundle.LoadFromFile(modFile);
                    if (loadedBundle == null)
                    {
                        Debug.LogWarning($"Unable to load mod '{modFile}', error loading asset bundle");
                        continue;
                    }

                    LSDRevampedMod[] mods = loadedBundle.LoadAllAssets<LSDRevampedMod>();
                    if (mods.Length > 1)
                    {
                        Debug.LogWarning($"Unable to load mod '{modFile}', multiple LSDRevampedMods in bundle");
                        continue;
                    }

                    if (mods.Length == 0)
                    {
                        Debug.LogWarning($"Unable to load mod '{modFile}', no LSDRevampedMod in bundle");
                        continue;
                    }

                    mods[0].SetSourceBundle(loadedBundle);
                    _loadedMods.Add(mods[0]);
                }
            }
        }

        protected void ensureDirectory()
        {
            // if the directory doesn't exist, create it
            if (!Directory.Exists(_modsDirectory)) Directory.CreateDirectory(_modsDirectory);
        }

        protected string getPlatformPathFragment()
        {
            return Application.platform switch
            {
                var r when r == RuntimePlatform.WindowsPlayer || r == RuntimePlatform.WindowsEditor => "windows",
                var r when r == RuntimePlatform.LinuxPlayer || r == RuntimePlatform.LinuxEditor => "linux",
                var r when r == RuntimePlatform.OSXPlayer || r == RuntimePlatform.OSXEditor => "mac",
                _ => throw new InvalidOperationException($"platform {Application.platform} not supported")
            };
        }
    }
}
