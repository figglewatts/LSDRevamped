using UnityEngine;

namespace LSDR.SDK.Audio
{
    public abstract class AbstractSongLibrary : ScriptableObject
    {
        public abstract SongAsset GetSong(SongStyle style, int songNumber);
    }
}
