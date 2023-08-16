using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Song")]
    public class SongAsset : ScriptableObject
    {
        public string Name;
        public string Author;
        public AudioClip Clip;
    }
}
