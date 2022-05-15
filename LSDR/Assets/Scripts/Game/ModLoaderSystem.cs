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
        public IEnumerable<LSDRevampedMod> Mods => _loadedMods;

        protected readonly List<LSDRevampedMod> _loadedMods = new List<LSDRevampedMod>();

        protected string _modsDirectory => Path.Combine(Application.streamingAssetsPath, "mods");

        public bool ModsAvailable => _loadedMods.Count > 0;

        public LSDRevampedMod GetMod(int modIdx)
        {
            if (modIdx >= _loadedMods.Count)
            {
                modIdx = _loadedMods.Count - 1;
            }
            else if (modIdx < 0)
            {
                modIdx = 0;
            }

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

            var modFiles = Directory.GetFiles(_modsDirectory, "*.lsdrmod", SearchOption.AllDirectories);
            foreach (var modFile in modFiles)
            {
                var bundleLoadRequest = AssetBundle.LoadFromFileAsync(modFile);
                yield return bundleLoadRequest;

                var mods = bundleLoadRequest.assetBundle.LoadAllAssets<LSDRevampedMod>();
                if (mods.Length > 1)
                {
                    Debug.LogWarning($"Unable to load mod '{modFile}', multiple LSDRevampedMods in bundle");
                    continue;
                }
                else if (mods.Length == 0)
                {
                    Debug.LogWarning($"Unable to load mod '{modFile}', no LSDRevampedMod in bundle");
                    continue;
                }

                _loadedMods.Add(mods[0]);
            }
        }
    }
}
