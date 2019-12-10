using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LSDR.Entities
{
    public class LevelEntities : MonoBehaviour
    {
        public Dictionary<Type, List<BaseEntity>> Entities;

        public void Awake()
        {
            Entities = new Dictionary<Type, List<BaseEntity>>();
        }

        public void Register<T>(T entity) where T : BaseEntity
        {
            var entityType = typeof(T);
            
            // first ensure that the list is initialized
            if (!Entities.ContainsKey(entityType))
            {
                Entities[entityType] = new List<BaseEntity>();
            }
            
            // now add the entity to the list
            Entities[entityType].Add(entity);
            entity.OnEntityDestroy += () => Deregister(entity);
            entity.LevelEntities = this;
        }

        public void Deregister<T>(T entity) where T : BaseEntity
        {
            var entityType = typeof(T);

            Entities[entityType].Remove(entity);
        }

        public List<T> OfType<T>() where T : BaseEntity
        {
            return Entities[typeof(T)].Select(item => item as T).ToList();
        }
    }
}
