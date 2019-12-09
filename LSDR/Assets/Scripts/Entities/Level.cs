using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using UnityEngine;

namespace LSDR.Entities
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic, SkipConstructor = true)]
    public class Level
    {
        public List<EntityMemento> Entities;
        
        [NonSerialized]
        private GameObject _levelObject;

        public Level(List<EntityMemento> entities) { Entities = entities; }

        public static Level FromScene(GameObject level)
        {
            var entities = level.GetComponentsInChildren<BaseEntity>().Select(e => e.Save()).ToList();
            return new Level(entities);
        }

        public GameObject ToScene()
        {
            _levelObject = new GameObject("Level");
            foreach (var entity in Entities)
            {
                GameObject entityObj = entity.CreateGameObject();
                entityObj.transform.SetParent(_levelObject.transform);
            }

            return _levelObject;
        }
    }
}
