using System.IO;
using libLSD.Formats;
using LSDR.SDK.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(version: 1, ext: "tix")]
    public class TIXImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // read the TIX file
            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(ctx.assetPath, FileMode.Open)))
            {
                tix = new TIX(br);
            }

            // convert it to a texture
            var texture = LibLSDUnity.GetTextureFromTIX(tix);
            var thumbnail = AssetPreview.GetMiniThumbnail(texture);

            // set the outputs
            ctx.AddObjectToAsset("TIX Texture", texture, thumbnail);
            ctx.SetMainObject(texture);
        }
    }
}
