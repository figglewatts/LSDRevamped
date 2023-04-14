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
        protected readonly List<LSDRevampedMod> _loadedMods = new List<LSDRevampedMod>();
        public IEnumerable<LSDRevampedMod> Mods => _loadedMods;

        protected string _modsDirectory => Path.Combine(Application.streamingAssetsPath, "mods");

        public bool ModsAvailable => _loadedMods.Count > 0;

        public LSDRevampedMod GetMod(int modIdx)
        {
            if (modIdx >= _loadedMods.Count)
                modIdx = _loadedMods.Count - 1;
            else if (modIdx < 0) modIdx = 0;

            return _loadedMods[modIdx];
        }

        public IEnumerator LoadMods()
        {
            Debug.Log("Loading mods...");

            if (_loadedMods.Count > 0)
            {
                Debug.LogWarning("Cannot load mods twice!");
                yield break;
            }

            string[] modFiles = Directory.GetFiles(_modsDirectory, "*.lsdrmod", SearchOption.AllDirectories);
            foreach (string modFile in modFiles)
            {
                AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(modFile);
                yield return bundleLoadRequest;

                AssetBundle loadedBundle = bundleLoadRequest.assetBundle;
                if (loadedBundle == null)
                {
                    Debug.LogWarning($"Unable to load mod '{modFile}', error loading asset bundle");
                    continue;
                }

                AssetBundleRequest assetLoadRequest =
                    bundleLoadRequest.assetBundle.LoadAllAssetsAsync<LSDRevampedMod>();
                yield return assetLoadRequest;

                Object[] mods = assetLoadRequest.allAssets;
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

                _loadedMods.Add((LSDRevampedMod)mods[0]);
            }
        }
    }
}
