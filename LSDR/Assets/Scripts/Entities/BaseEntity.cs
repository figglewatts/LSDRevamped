using System;
using LSDR.Entities.Dream;
using ProtoBuf;
using UnityEngine;

namespace LSDR.Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        public string EntityID = Guid.NewGuid().ToString();
        public System.Action OnEntityDestroy;

        public LevelEntities LevelEntities;

        public abstract EntityMemento Save();

        public virtual void Restore(EntityMemento memento, LevelEntities entities)
        {
            EntityID = memento.EntityID;
            transform.position = memento.Position;
            transform.rotation = memento.Rotation;
            transform.localScale = memento.Scale;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(0.25f, 0.25f, 0.25f));
        }

        public void OnDestroy() { OnEntityDestroy(); }
    }
}
