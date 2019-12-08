using System;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class SpawnPoint : MonoBehaviour
    {
        public void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "SpawnPoint.png");
        }
    }
}
