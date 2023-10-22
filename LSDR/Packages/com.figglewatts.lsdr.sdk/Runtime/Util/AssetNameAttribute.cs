using UnityEngine;

namespace LSDR.SDK.Util
{
    public class AssetNameAttribute : PropertyAttribute
    {
        public System.Type AssetType { get; private set; }
        public bool UseResourcesPath { get; private set; }

        public AssetNameAttribute(System.Type assetType, bool useResourcesPath = false)
        {
            AssetType = assetType;
            UseResourcesPath = useResourcesPath;
        }
    }
}
