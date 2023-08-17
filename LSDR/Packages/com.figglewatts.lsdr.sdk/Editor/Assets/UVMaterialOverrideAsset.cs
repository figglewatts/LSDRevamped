using System.Collections.Generic;
using LSDR.SDK.Assets;
using UnityEngine;

namespace LSDR.SDK.Editor.Assets
{
    [CreateAssetMenu(menuName = "LSDR SDK/UV Material Override")]
    public class UVMaterialOverrideAsset : ScriptableObject
    {
        public List<UVMaterialOverride> Overrides;
    }
}
