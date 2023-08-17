using System.Collections.Generic;
using libLSD.Formats;
using UnityEngine;

namespace LSDR.SDK.Assets
{
    public class VABAsset : ScriptableObject
    {
        public List<AudioClip> Samples;
        [HideInInspector] public VAB Vab;
    }
}
