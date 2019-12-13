using System;
using LSDR.Dream;
using LSDR.Entities;
using Torii.Serialization;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName="System/LevelLoaderSystem")]
    public class LevelLoaderSystem : ScriptableObject
    {
        public Action<LevelEntities> OnLevelLoaded;
        public DreamSystem DreamSystem;
        public SettingsSystem SettingsSystem;
        
        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        public GameObject LoadLevel(string levelPath)
        {
            Level level = _serializer.Deserialize<Level>(levelPath);
            LevelEntities entities = level.ToScene(DreamSystem, SettingsSystem);
            OnLevelLoaded(entities);
            return entities.gameObject;
        }
    }
}
