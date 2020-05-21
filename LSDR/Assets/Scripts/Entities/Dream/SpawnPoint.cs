using System;
using MapParse.Types;
using ProtoBuf;
using Torii.Event;
using Torii.Resource;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class SpawnPoint : BaseEntity
    {
        public bool DayOneSpawn;
        public bool TunnelEntrance;
        
        [NonSerialized]
        public GameObject PlayerPrefab;

        public void Awake() { PlayerPrefab = ResourceManager.UnityLoad<GameObject>("Prefabs/Player"); }

        public void Spawn()
        {
            Debug.Log("Instantiating player");
            GameObject player = Instantiate(PlayerPrefab, transform.position + new Vector3(0, 0.23f, 0),
                transform.rotation);
            DreamSystem.SpawnPlayer(player);
        }

        public void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawIcon(position, "SpawnPoint.png");
            Gizmos.DrawRay(position, transform.forward * 0.25f);
        }

        public override EntityMemento Save() { return new SpawnPointMemento(this); }

        public override void Restore(EntityMemento memento, LevelEntities entities)
        {
            base.Restore(memento, entities);

            var spawnPointMemento = (SpawnPointMemento)memento;
            DayOneSpawn = spawnPointMemento.DayOneSpawn;
            TunnelEntrance = spawnPointMemento.TunnelEntrance;
            entities.Register(this);
        }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic, SkipConstructor = true)]
    public class SpawnPointMemento : EntityMemento
    {
        public bool DayOneSpawn;
        public bool TunnelEntrance;

        protected override Type EntityType => typeof(SpawnPoint);

        public SpawnPointMemento(SpawnPoint state) : base(state)
        {
            DayOneSpawn = state.DayOneSpawn;
            TunnelEntrance = state.TunnelEntrance;
        }
    }
}
