using System;
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

        public void Build(LSDRevampedMod mod, string outputPath)
        {
            var assetNames =
                new[] { AssetDatabase.GetAssetPath(mod) }.Concat(mod.Journals.SelectMany(j => j.Dreams)
                                                                    .Select(d => d.DreamPrefabPath));
            AssetBundleBuild build = new AssetBundleBuild
            {
                assetNames = assetNames.ToArray(),
                assetBundleName = $"{mod.Name}.lsdrmod"
            };

            Debug.Log($"Building mod {mod.Name} for Windows...");
            buildForPlatform(BuildTarget.StandaloneWindows64, build, outputPath);

            Debug.Log($"Building mod {mod.Name} for Linux...");
            buildForPlatform(BuildTarget.StandaloneLinux64, build, outputPath);

            Debug.Log($"Building mod {mod.Name} for macOS (OS X)...");
            buildForPlatform(BuildTarget.StandaloneOSX, build, outputPath);

            EditorUtility.DisplayDialog("Mod finished building", "Mod built successfully!", "Ok");
            EditorUtility.RevealInFinder(Path.Combine(outputPath, $"{mod.Name}.lsdrmod"));
        }

        protected void buildForPlatform(BuildTarget platform, AssetBundleBuild bundle, string outputPath)
        {
            string platformPathSegment = platformToModPathSegment(platform);
            string fullOutputDirectory = Path.Combine(outputPath, platformPathSegment);
            Directory.CreateDirectory(fullOutputDirectory);
            BuildPipeline.BuildAssetBundles(fullOutputDirectory, new[] { bundle }, BuildOptions, platform);

            string weirdPath = Path.Combine(fullOutputDirectory, $"{platformPathSegment}");
            string manifestPath = Path.Combine(fullOutputDirectory, $"{platformPathSegment}.manifest");
            string additionalManifestPath = Path.Combine(fullOutputDirectory, $"{bundle.assetBundleName}.manifest");
            File.Delete(manifestPath);
            File.Delete(weirdPath);
            File.Delete(additionalManifestPath);
        }

        protected string platformToModPathSegment(BuildTarget platform)
        {
            return platform switch
            {
                BuildTarget.StandaloneWindows64 => "windows",
                BuildTarget.StandaloneLinux64 => "linux",
                BuildTarget.StandaloneOSX => "mac",
                _ => throw new ArgumentException("unsupported build target " + platform, nameof(platform))
            };
        }
    }
}
