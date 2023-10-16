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
        public List<LSDRevampedMod> BuiltInMods;

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

        public void LoadMods()
        {
            Debug.Log("Loading mods...");

            if (_loadedMods.Count > 0)
            {
                Debug.LogWarning("Cannot load mods twice!");
                return;
            }

            ensureDirectory();

            _loadedMods.AddRange(BuiltInMods);

            string[] modFiles = Directory.GetFiles(_modsDirectory, "*.lsdrmod", SearchOption.AllDirectories);
            foreach (string modFile in modFiles)
            {
                AssetBundle loadedBundle = AssetBundle.LoadFromFile(modFile);
                if (loadedBundle == null)
                {
                    Debug.LogWarning($"Unable to load mod '{modFile}', error loading asset bundle");
                    continue;
                }

                Object[] mods = loadedBundle.LoadAllAssets();
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

        protected void ensureDirectory()
        {
            // if the directory doesn't exist, create it
            if (!Directory.Exists(_modsDirectory)) Directory.CreateDirectory(_modsDirectory);
        }
    }
}
