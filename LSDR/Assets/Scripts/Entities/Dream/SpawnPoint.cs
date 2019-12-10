using System;
using MapParse.Types;
using ProtoBuf;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class SpawnPoint : BaseEntity
    {
        public bool DayOneSpawn;

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
            DayOneSpawn = spawnPointMemento.DayOneSpawn;
        }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic, SkipConstructor = true)]
    public class SpawnPointMemento : EntityMemento
    {
        public bool DayOneSpawn;

        protected override Type EntityType => typeof(SpawnPoint);
        public SpawnPointMemento(SpawnPoint state) : base(state) { DayOneSpawn = state.DayOneSpawn; }
    }
}
