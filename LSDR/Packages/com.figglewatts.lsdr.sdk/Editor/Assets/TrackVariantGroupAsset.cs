using System.Collections.Generic;
using LSDR.SDK.Assets;
using UnityEngine;

namespace LSDR.SDK.Editor.Assets
{
    /// <summary>
    /// Used for grouping together the soundfonts to render the variants of a SEQ clip in when importing.
    /// </summary>
    [CreateAssetMenu(menuName = "LSDR SDK/Track variant group")]
    public class TrackVariantGroupAsset : ScriptableObject
    {
        public List<VABAsset> SoundfontVariants;
    }
}
