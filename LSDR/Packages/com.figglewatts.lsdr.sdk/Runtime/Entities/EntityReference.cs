using System;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class EntityReference : MonoBehaviour
    {
        public string ReferenceID = Guid.NewGuid().ToString();
    }
}
