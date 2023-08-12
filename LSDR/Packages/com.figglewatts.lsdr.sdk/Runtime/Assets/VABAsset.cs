using System.Collections.Generic;
using libLSD.Formats;
using UnityEngine;

namespace LSDR.SDK.Assets
{
    public class VABAsset : ScriptableObject
    {
        [HideInInspector] public VAB Vab;
        public List<AudioClip> Samples;
    }
}
