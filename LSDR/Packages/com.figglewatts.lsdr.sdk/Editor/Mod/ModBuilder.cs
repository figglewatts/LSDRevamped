using System.Collections.Generic;
using System.IO;
using System.Linq;
using LSDR.SDK.Data;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Mod
{
    public class ModBuilder
    {
        protected const BuildAssetBundleOptions BuildOptions = BuildAssetBundleOptions.StrictMode |
                                                               BuildAssetBundleOptions.ChunkBasedCompression |
                                                               BuildAssetBundleOptions.AssetBundleStripUnityVersion |
                                                               BuildAssetBundleOptions.ForceRebuildAssetBundle;

        public void Build(LSDRevampedMod mod, ModPlatform platforms, string outputPath)
        {
            if (platforms == ModPlatform.Nothing)
            {
                Debug.LogWarning("No platforms supplied to mod builder, doing nothing.");
                return;
            }
            
            AssetBundleBuild build = new AssetBundleBuild
            {
                assetNames = new [] {AssetDatabase.GetAssetPath(mod)},
                assetBundleName = $"{mod.Name}.lsdrmod"
            };

            if (platforms.HasFlag(ModPlatform.Windows))
            {
                Debug.Log($"Building mod {mod.Name} for Windows...");
                buildForPlatform(BuildTarget.StandaloneWindows64, build, outputPath);
            }

            if (platforms.HasFlag(ModPlatform.Linux))
            {
                Debug.Log($"Building mod {mod.Name} for Linux...");
                buildForPlatform(BuildTarget.StandaloneLinux64, build, outputPath);
            }

            if (platforms.HasFlag(ModPlatform.OSX))
            {
                Debug.Log($"Building mod {mod.Name} for macOS (OS X)...");
                buildForPlatform(BuildTarget.StandaloneOSX, build, outputPath);
            }
            
            Debug.Log("Mod built successfully!");
        }

        protected void buildForPlatform(BuildTarget platform, AssetBundleBuild bundle, string outputPath)
        {
            var fullOutputDirectory = Path.Combine(outputPath, platform.ToString());
            Directory.CreateDirectory(fullOutputDirectory);
            BuildPipeline.BuildAssetBundles(fullOutputDirectory, new[] { bundle }, BuildOptions, platform);

            var manifestBundlePath = Path.Combine(fullOutputDirectory, platform.ToString());
            File.Delete(manifestBundlePath);
            File.Delete($"{manifestBundlePath}.manifest");
        }
    }
}
