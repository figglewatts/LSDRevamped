using System.IO;
using LSDR.SDK.Assets;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(version: 1, "lua")]
    public class LuaScriptImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            LuaScriptAsset luaScriptAsset = ScriptableObject.CreateInstance<LuaScriptAsset>();
            luaScriptAsset.ScriptText = File.ReadAllText(ctx.assetPath);

            ctx.AddObjectToAsset("Script", luaScriptAsset);
            ctx.SetMainObject(luaScriptAsset);
        }
    }
}
