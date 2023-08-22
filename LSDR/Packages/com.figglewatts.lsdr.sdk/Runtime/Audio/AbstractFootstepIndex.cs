using UnityEngine;

namespace LSDR.SDK.Audio
{
    public abstract class AbstractFootstepIndex : ScriptableObject
    {
        public Footstep Fallback;

        public abstract Footstep GetFootstep(RaycastHit hit);
    }
}
