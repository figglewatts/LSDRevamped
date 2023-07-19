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
            AssetBundleBuild build = new AssetBundleBuild
            {
                assetNames = new[] { AssetDatabase.GetAssetPath(mod) },
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

            EditorUtility.DisplayDialog("Mod finished building", "Mod built successfully!", "Ok");
            EditorUtility.RevealInFinder(Path.Combine(outputPath, $"{mod.Name}.lsdrmod"));
        }

        protected void buildForPlatform(BuildTarget platform, AssetBundleBuild bundle, string outputPath)
        {
            var fullOutputDirectory = outputPath;
            Directory.CreateDirectory(fullOutputDirectory);
            BuildPipeline.BuildAssetBundles(fullOutputDirectory, new[] { bundle }, BuildOptions, platform);

            var manifestPath = Path.Combine(fullOutputDirectory, $"{bundle.assetBundleName}.manifest");
            var outputDirName = new DirectoryInfo(outputPath).Name;
            var weirdPath = Path.Combine(fullOutputDirectory, outputDirName);
            var weirdPathManifest = $"{weirdPath}.manifest";
            File.Delete(manifestPath);
            File.Delete(weirdPath);
            File.Delete(weirdPathManifest);
        }
    }
}
