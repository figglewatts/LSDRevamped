using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Material Footstep Index")]
    public class MaterialFootstepIndex : BaseFootstepIndex<Material>
    {
        public override Footstep GetFootstep(RaycastHit hit)
        {
            var meshRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();

            if (!Index.ContainsKey(meshRenderer.sharedMaterial)) return Fallback;
            return Index[meshRenderer.sharedMaterial];
        }
    }
}
