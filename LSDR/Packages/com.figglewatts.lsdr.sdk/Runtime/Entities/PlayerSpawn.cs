using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class PlayerSpawn : MonoBehaviour
    {
        [Tooltip("Should this spawn point be used as the spawn point on day 1?")]
        public bool DayOneSpawn;

        [Tooltip("Is this spawn point a tunnel entrance?")]
        public bool TunnelEntrance;
    }
}
