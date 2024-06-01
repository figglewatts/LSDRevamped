using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/UV Footstep Index")]
    public class UVFootstepIndex : BaseFootstepIndex<RectInt>
    {
        public Vector2Int AtlasDimensions;

        public override Footstep GetFootstep(RaycastHit hit)
        {
            var hitUv = hit.textureCoord;
            hitUv.y = 1f - hitUv.y; // invert Y so we work from image pixel coords (top right)
            Vector2 hitUvPixels = hitUv * AtlasDimensions;
            Vector2Int hitUvPixelsInt = new Vector2Int((int)hitUvPixels.x, (int)hitUvPixels.y);

            foreach (var uvRect in Index.Keys)
            {
                if (uvRect.Contains(hitUvPixelsInt))
                {
                    return Index[uvRect];
                }
            }
            return Fallback;
        }
    }
}
