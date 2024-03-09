using System.Collections.Generic;
using LSDR.SDK.Assets;
using UnityEngine;

namespace LSDR.SDK.Editor.Assets
{
    /// <summary>
    /// Used to override areas of the UVs of TMD models when loading with a different material than the one
    /// from the TMD file. Used for replacing the water from the original LBDs with the water shader.
    /// </summary>
    [CreateAssetMenu(menuName = "LSDR SDK/UV Material Override")]
    public class UVMaterialOverrideAsset : ScriptableObject
    {
        public List<UVMaterialOverride> Overrides;
    }
}
