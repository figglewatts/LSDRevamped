using System.IO;
using libLSD.Formats;
using UnityEditor;

using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [UnityEditor.AssetImporters.ScriptedImporter(version: 1, "tix")]
    public class TIXImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            // read the TIX file
            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(ctx.assetPath, FileMode.Open)))
            {
                tix = new TIX(br);
            }

            // convert it to a texture
            Texture2D texture = LibLSDUnity.GetTextureFromTIX(tix);
            Texture2D thumbnail = AssetPreview.GetMiniThumbnail(texture);

            // set the outputs
            ctx.AddObjectToAsset("TIX Texture", texture, thumbnail);
            ctx.SetMainObject(texture);
        }
    }
}
