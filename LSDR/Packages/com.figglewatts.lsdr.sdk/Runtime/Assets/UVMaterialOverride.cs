using System;
using UnityEngine;

namespace LSDR.SDK.Assets
{
    [Serializable]
    public struct UVMaterialOverride
    {
        public Rect UVRect;
        public Material Material;

        public Vector2 MapUVInRect(Vector2 uv)
        {
            if (!UVRect.Contains(uv))
            {
                throw new InvalidOperationException($"uv {uv} is outside rect {UVRect}");
            }

            return (uv - UVRect.position) / UVRect.size;
        }
    }
}
