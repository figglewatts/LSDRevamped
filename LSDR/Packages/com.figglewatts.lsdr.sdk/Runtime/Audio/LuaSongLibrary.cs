using LSDR.SDK.Assets;
using LSDR.SDK.Lua;
using UnityEngine;
using UnityEngine.Serialization;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Lua Song Library")]
    public class LuaSongLibrary : AbstractSongLibrary
    {
        public SongListAsset SongList;
        public LuaScriptAsset Script;

        public override SongAsset GetSong(SongStyle style, int songNumber)
        {
            if (Script == null)
            {
                Debug.LogError("Attempted to GetSong from LuaSongLibrary without a LuaScriptAsset");
                return null;
            }

            SongLibraryLuaScript loadedScript = new SongLibraryLuaScript(LuaManager.Managed, Script, SongList);
            return loadedScript.GetSong(style, songNumber);
        }
    }
}
