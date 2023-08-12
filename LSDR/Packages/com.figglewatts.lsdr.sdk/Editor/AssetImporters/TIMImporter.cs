using System.IO;
using libLSD.Formats;
using LSDR.SDK.Assets;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(version: 1, "tim")]
    public class TIMImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // read the TIM file
            TIM tim;
            using (BinaryReader br = new BinaryReader(File.Open(ctx.assetPath, FileMode.Open)))
            {
                tim = new TIM(br);
            }

            // create the TIM asset
            TIMAsset timAsset = ScriptableObject.CreateInstance<TIMAsset>();

            string assetName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            // load the palettes
            int numberOfCLUTs = tim.ColorLookup?.NumberOfCLUTs ?? 1;
            timAsset.Palettes = new Texture2D[numberOfCLUTs];
            for (int i = 0; i < numberOfCLUTs; i++)
            {
                Texture2D paletteTex = LibLSDUnity.GetTextureFromTIM(tim, i);
                timAsset.Palettes[i] = paletteTex;

                Texture2D paletteThumbnail = AssetPreview.GetMiniThumbnail(paletteTex);

                if (i == 0)
                {
                    // add the main asset, with the thumbnail of the first palette
                    ctx.AddObjectToAsset("TIM", timAsset, paletteThumbnail);
                    ctx.SetMainObject(timAsset);
                }

                paletteTex.name = $"{assetName} Palette {i}";
                ctx.AddObjectToAsset($"TIM Texture {i}", paletteTex, paletteThumbnail);
            }
        }
    }
}
