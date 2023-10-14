using System;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        public string ID = Guid.NewGuid().ToString();

        public virtual void Init() { }

        public void Awake()
        {
            ID = ID.Replace("$$", Guid.NewGuid().ToString());
            name = ID;
            EntityIndex.Instance.Register(this);
        }

        public virtual void OnValidate()
        {
            name = ID;
        }
    }
}
