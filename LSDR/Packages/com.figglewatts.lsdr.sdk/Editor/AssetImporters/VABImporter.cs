using System.Collections.Generic;
using System.IO;
using System.Linq;
using libLSD.Formats;
using LSDR.SDK.Assets;
using LSDR.SDK.Editor.Util;
using LSDR.SDK.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [UnityEditor.AssetImporters.ScriptedImporter(version: 1, "vh")]
    public class VABImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public int SampleRate = 11025;

        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            var vhFilePath = ctx.assetPath;
            var vbFilePath = Path.ChangeExtension(vhFilePath, ".vb");

            VAB vab;
            using (BinaryReader vhBr = new BinaryReader(File.OpenRead(vhFilePath)))
            {
                using (BinaryReader vbBr = new BinaryReader(File.OpenRead(vbFilePath)))
                {
                    vab = new VAB(vhBr, vbBr);
                }
            }

            VABAsset vabAsset = ScriptableObject.CreateInstance<VABAsset>();
            string assetName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            ctx.AddObjectToAsset(assetName, vabAsset);
            ctx.SetMainObject(vabAsset);
            vabAsset.name = assetName;
            vabAsset.Vab = vab;
            loadSamples(vabAsset, ctx.assetPath);
        }

        protected void loadSamples(VABAsset vabAsset, string assetPath)
        {
            var assetDirectory = new DirectoryInfo(assetPath).Parent!.FullName;

            // load the VAB
            string fullOutputPath = Path.Combine(assetDirectory, "LoadedSamples", vabAsset.name);
            Directory.CreateDirectory(fullOutputPath);
            string[] sampleAssetPaths = new string[vabAsset.Vab.Samples.Count];

            // write each sample to a WAV file
            int sampleNum = 0;
            foreach (var sample in vabAsset.Vab.Samples)
            {
                string samplePathName = $"Sample{sampleNum}.wav";
                string samplePath = Path.Combine(fullOutputPath, samplePathName);
                WaveUtil.WriteWave(samplePath, sample.SampleData, 1, SampleRate);

                string sampleAssetPath = DirectoryUtil.MakePathAssetsRelative(samplePath);
                AssetDatabase.ImportAsset(sampleAssetPath);
                sampleAssetPaths[sampleNum] = sampleAssetPath;

                sampleNum++;
            }

            // hook up the samples
            EditorApplication.delayCall += () =>
            {
                VABAsset vabAsset = AssetDatabase.LoadAssetAtPath<VABAsset>(assetPath);
                foreach (var sampleAssetPath in sampleAssetPaths)
                {
                    AudioClip importedSample = AssetDatabase.LoadAssetAtPath<AudioClip>(sampleAssetPath);
                    vabAsset.Samples.Add(importedSample);
                }
                EditorUtility.SetDirty(vabAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
        }
    }
}
