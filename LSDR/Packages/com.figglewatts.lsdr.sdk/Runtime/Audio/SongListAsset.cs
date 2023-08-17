using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Song List")]
    public class SongListAsset : ScriptableObject
    {
        public List<SongAsset> Songs;
    }
}
