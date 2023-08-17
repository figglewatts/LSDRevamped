using System.IO;
using LSDR.SDK.Assets;

using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [UnityEditor.AssetImporters.ScriptedImporter(version: 1, "lua")]
    public class LuaScriptImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            LuaScriptAsset luaScriptAsset = ScriptableObject.CreateInstance<LuaScriptAsset>();
            luaScriptAsset.ScriptText = File.ReadAllText(ctx.assetPath);

            ctx.AddObjectToAsset("Script", luaScriptAsset);
            ctx.SetMainObject(luaScriptAsset);
        }
    }
}
