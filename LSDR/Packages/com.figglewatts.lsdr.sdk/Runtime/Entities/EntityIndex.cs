using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class EntityIndex : MonoBehaviour
    {
        public static EntityIndex Instance
        {
            get
            {
                // if it was already created, return it
                if (_instance) return _instance;

                // if the object exists but we don't know about it, get it and return it
                _instance = FindObjectOfType<EntityIndex>();
                if (_instance) return _instance;

                // if the object doesn't exist, create it
                _instance = new GameObject("EntityIndex", typeof(EntityIndex)).GetComponent<EntityIndex>();
                DontDestroyOnLoad(_instance);
                return _instance;
            }
        }

        protected static EntityIndex _instance;

        protected readonly Dictionary<string, GameObject> _entities = new Dictionary<string, GameObject>();

        public void Register(BaseEntity entity)
        {
            Register(entity.ID, entity.gameObject);
        }

        public void Register(string id, GameObject entity)
        {
            if (_entities.ContainsKey(id))
            {
                Debug.LogError($"Unable to register entity '{entity}': duplicate entity ID '{id}'");
                return;
            }

            _entities[id] = entity;
        }

        public GameObject Get(string id)
        {
            if (!_entities.ContainsKey(id))
            {
                Debug.LogError($"No entity with ID '{id}'");
                return null;
            }

            return _entities[id];
        }

        public T GetComponent<T>(string id) where T : Component
        {
            var obj = Get(id);
            var component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Entity with ID '{id}' did not have component of type {typeof(T)}");
                return null;
            }
            return component;
        }

        public void DeregisterAllEntities() => _entities.Clear();
    }
}
