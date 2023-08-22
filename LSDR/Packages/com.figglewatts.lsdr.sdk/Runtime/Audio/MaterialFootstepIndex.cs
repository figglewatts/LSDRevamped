using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Material Footstep Index")]
    public class MaterialFootstepIndex : BaseFootstepIndex<Material>
    {
        public override Footstep GetFootstep(RaycastHit hit)
        {
            throw new System.NotImplementedException();
        }
    }
}
