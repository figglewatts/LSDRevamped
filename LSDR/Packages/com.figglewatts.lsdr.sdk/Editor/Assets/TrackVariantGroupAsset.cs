using System.Collections.Generic;
using LSDR.SDK.Assets;
using UnityEngine;

namespace LSDR.SDK.Editor.Assets
{
    [CreateAssetMenu(menuName = "LSDR SDK/Track variant group")]
    public class TrackVariantGroupAsset : ScriptableObject
    {
        public List<VABAsset> SoundfontVariants;
    }
}
