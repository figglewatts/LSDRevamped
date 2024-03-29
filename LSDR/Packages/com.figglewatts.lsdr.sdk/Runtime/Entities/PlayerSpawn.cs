using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class PlayerSpawn : BaseEntity
    {
        [Tooltip("Should this spawn point be used as the spawn point on day 1?")]
        public bool DayOneSpawn;

        [Tooltip("Is this spawn point a tunnel entrance?")]
        public bool TunnelEntrance;

        public void Spawn(Transform target, bool setOrientation = false)
        {
            CharacterController controller = target.GetComponent<CharacterController>();
            float skinWidth = 0f;
            if (controller) skinWidth = controller.skinWidth;

            target.transform.position =
                new Vector3(transform.position.x + 0.01f, transform.position.y + skinWidth,
                    transform.position.z - 0.01f);
            if (setOrientation)
                target.transform.rotation = Quaternion.Euler(x: 0, transform.rotation.eulerAngles.y, z: 0);
        }
    }
}
