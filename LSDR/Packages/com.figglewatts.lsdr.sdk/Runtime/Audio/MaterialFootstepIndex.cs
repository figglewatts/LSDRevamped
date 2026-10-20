using LSDR.SDK.Util;
using LSDR.SDK.Visual;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Material Footstep Index")]
    public class MaterialFootstepIndex : BaseFootstepIndex<Material>
    {
        public override Footstep GetFootstep(RaycastHit hit)
        {
            var meshRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
            var material = TextureSetter.Instance.GetSourceMaterial(meshRenderer.sharedMaterial);

            if (!Index.ContainsKey(material)) return Fallback;
            return Index[material];
        }
    }
}
