using System;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        public string ID = Guid.NewGuid().ToString();

        public virtual void Init() { }

        public void Awake() { EntityIndex.Instance.Register(this); }

        public virtual void OnValidate()
        {
            name = ID;
        }
    }
}
