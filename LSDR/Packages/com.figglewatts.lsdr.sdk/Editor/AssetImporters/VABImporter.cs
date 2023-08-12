using System.Collections.Generic;
using System.IO;
using System.Linq;
using libLSD.Formats;
using LSDR.SDK.Assets;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(version: 1, "vh")]
    public class VABImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
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
            vabAsset.Vab = vab;
            vabAsset.Samples = new List<AudioClip>();

            int sampleNumber = 0;
            foreach (var sample in vab.Samples)
            {
                string clipName = $"{assetName}Sample{sampleNumber}";
                AudioClip audioClip = AudioClip.Create(clipName,
                    sample.SampleData.Length, 1,
                    44100, false);
                var sampleData = sample.SampleData.Select(s => (s / (float)short.MaxValue) / 2).ToArray();
                audioClip.SetData(sampleData, 0);
                audioClip.LoadAudioData();
                vabAsset.Samples.Add(audioClip);
                ctx.AddObjectToAsset(clipName, audioClip);

                sampleNumber++;
            }
        }
    }
}
