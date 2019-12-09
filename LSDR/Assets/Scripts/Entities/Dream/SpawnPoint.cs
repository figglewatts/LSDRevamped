using System;
using MapParse.Types;
using ProtoBuf;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class SpawnPoint : BaseEntity
    {
        public bool Test;

        public void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawIcon(position, "SpawnPoint.png");
            Gizmos.DrawRay(position, transform.forward * 0.25f);
        }

        public override EntityMemento Save() { return new SpawnPointMemento(this); }

        public override void Restore(EntityMemento memento)
        {
            base.Restore(memento);

            var spawnPointMemento = (SpawnPointMemento)memento;
            Test = spawnPointMemento.Test;
        }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic, SkipConstructor = true)]
    public class SpawnPointMemento : EntityMemento
    {
        public bool Test;

        protected override Type EntityType => typeof(SpawnPoint);
        public SpawnPointMemento(SpawnPoint state) : base(state) { Test = state.Test; }
    }
}
