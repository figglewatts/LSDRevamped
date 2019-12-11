using System;
using LSDR.Entities;
using Torii.Serialization;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName="System/LevelLoaderSystem")]
    public class LevelLoaderSystem : ScriptableObject
    {
        public Action<LevelEntities> OnLevelLoaded;
        
        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        public GameObject LoadLevel(string levelPath)
        {
            Level level = _serializer.Deserialize<Level>(levelPath);
            LevelEntities entities = level.ToScene();
            OnLevelLoaded(entities);
            return entities.gameObject;
        }
    }
}
