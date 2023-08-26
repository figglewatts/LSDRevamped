using System;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        public string ID = Guid.NewGuid().ToString();

        public virtual void Start() { EntityIndex.Instance.Register(this); }

        public virtual void OnValidate()
        {
            name = ID;
        }
    }
}
