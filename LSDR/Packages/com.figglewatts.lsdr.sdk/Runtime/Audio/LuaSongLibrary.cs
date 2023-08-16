using LSDR.SDK.Assets;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Lua Song Library")]
    public class LuaSongLibrary : AbstractSongLibrary
    {
        public SongListAsset Songs;
        public LuaScriptAsset Script;

        public override SongAsset GetSong(SongStyle style, int songNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
