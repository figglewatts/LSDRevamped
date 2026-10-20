using System;
using System.Collections.Generic;
using System.IO;
using libLSD.Audio.Playback;
using libLSD.Formats;
using LSDR.SDK.Assets;
using LSDR.SDK.Editor.Assets;
using LSDR.SDK.Editor.Util;
using LSDR.SDK.Util;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    // imported after VABs, as rendering the tracks needs them
    [ScriptedImporter(3, "SEQ", importQueueOffset: 100)]
    public class SEQImporter : ScriptedImporter
    {
        public TrackVariantGroupAsset VariantGroup;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using FileStream fs = new FileStream(ctx.assetPath, FileMode.Open);
            using BinaryReader br = new BinaryReader(fs);
            SEQ seq = new SEQ(br);

            SEQAsset seqAsset = ScriptableObject.CreateInstance<SEQAsset>();
            string assetName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            ctx.AddObjectToAsset("seq", seqAsset);
            ctx.SetMainObject(seqAsset);
            seqAsset.name = assetName;

            if (VariantGroup != null) loadTracks(seqAsset, seq, ctx.assetPath);
        }

        protected void loadTracks(SEQAsset seqAsset, SEQ seq, string path)
        {
            var assetDirectory = new DirectoryInfo(path).Parent!.FullName;

            string fullOutputPath = Path.Combine(assetDirectory, "LoadedTracks", seqAsset.name);
            Directory.CreateDirectory(fullOutputPath);
            List<string> loadedTrackAssetPaths = new List<string>(VariantGroup.SoundfontVariants.Count);

            // write each track variation to a WAV file
            foreach (var soundfontVariant in VariantGroup.SoundfontVariants)
            {
                string trackPathName = $"{seqAsset.name}-{soundfontVariant.name}.wav";
                string trackPath = Path.Combine(fullOutputPath, trackPathName);

                string trackAssetPath = DirectoryUtil.MakePathAssetsRelative(trackPath);
                loadedTrackAssetPaths.Add(trackAssetPath);

                if (!File.Exists(trackPath))
                {
                    // the imported VAB asset doesn't keep everything needed to play it, so read it from disk
                    VAB vab = VABImporter.LoadVAB(AssetDatabase.GetAssetPath(soundfontVariant));
                    int[] loop = SEQPlayer.RenderLoop(seq, vab);

                    // stacked voices can reach twice full scale, so halve it
                    short[] trackSamples = new short[loop.Length];
                    for (int i = 0; i < loop.Length; i++)
                    {
                        trackSamples[i] = (short)Math.Clamp(loop[i] / 2, short.MinValue, short.MaxValue);
                    }
                    WaveUtil.WriteWave(trackPath, trackSamples, 2, SPU.SAMPLE_RATE);

                    AssetDatabase.ImportAsset(trackAssetPath);
                }
            }

            // hook up the track variations
            EditorApplication.delayCall += () =>
            {
                SEQAsset seqAsset = AssetDatabase.LoadAssetAtPath<SEQAsset>(path);
                foreach (var trackAssetPath in loadedTrackAssetPaths)
                {
                    AudioClip importedTrack = AssetDatabase.LoadAssetAtPath<AudioClip>(trackAssetPath);
                    seqAsset.Variations.Add(importedTrack);
                }
                EditorUtility.SetDirty(seqAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
        }
    }
}
