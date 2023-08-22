using LSDR.SDK.Audio;
using LSDR.SDK.Entities;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/LBD Footstep Index")]
    public class LBDFootstepIndex : BaseFootstepIndex<int>
    {
        public override Footstep GetFootstep(RaycastHit hit)
        {
            var gameObject = hit.collider.gameObject;
            var hitPositionWorldspace = hit.point;

            var lbdChunk = gameObject.GetComponent<LBDChunk>();
            if (lbdChunk == null) return Fallback;

            int xPos = (int)(hitPositionWorldspace.x - lbdChunk.transform.position.x + 0.5f);
            int yPos = (int)(hitPositionWorldspace.z - lbdChunk.transform.position.z + 0.5f);

            Debug.Log($"lbd footstep on: {xPos}, {yPos}");

            if (xPos < 0 || xPos >= 20 || yPos < 0 || yPos >= 20) return Fallback;

            int footstepKey = lbdChunk.Tiles[xPos * 20 + yPos].FootstepSoundAndCollision & 0x7F;

            if (!Index.ContainsKey(footstepKey)) return Fallback;
            return Index[footstepKey];
        }
    }
}
