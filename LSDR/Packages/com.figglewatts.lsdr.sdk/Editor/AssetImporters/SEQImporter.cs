using System;
using System.Collections.Generic;
using System.IO;
using EZSynth.Sequencer;
using EZSynth.Synthesizer;
using libLSD.Audio.Sequence;
using libLSD.Audio.Soundbank;
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
    [ScriptedImporter(1, "SEQ")]
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

            // create the sequence for the synthesizer
            SEQSequence seqSequence = new SEQSequence(seq);

            // write each track variation to a WAV file
            foreach (var soundfontVariant in VariantGroup.SoundfontVariants)
            {
                string trackPathName = $"{seqAsset.name}-{soundfontVariant.name}.wav";
                string trackPath = Path.Combine(fullOutputPath, trackPathName);

                string trackAssetPath = DirectoryUtil.MakePathAssetsRelative(trackPath);
                loadedTrackAssetPaths.Add(trackAssetPath);

                if (!File.Exists(trackPath))
                {
                    // create the synth stuff to render the track
                    VABSoundbank vabSoundbank = new VABSoundbank(soundfontVariant.Vab);
                    if (soundfontVariant.name.Equals("AMBIENT", StringComparison.InvariantCulture))
                        vabSoundbank.LongRelease = true;
                    Synth synth = new Synth(vabSoundbank);
                    Sequencer sequencer = new Sequencer(seqSequence, synth);

                    // render and save the track
                    short[] trackSamples = sequencer.Render();
                    WaveUtil.WriteWave(trackPath, trackSamples, 2, 44100);

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
