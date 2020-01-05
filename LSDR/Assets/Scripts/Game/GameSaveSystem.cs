using System;
using System.IO;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName="System/GameSaveSystem")]
    public class GameSaveSystem : ScriptableObject
    {
        public JournalLoaderSystem JournalLoader;
        
        public GameSaveData Data { get; private set; }
        
        private string _savedGamePath;
        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        public GameSaveData.JournalSaveData CurrentJournalSave => Data.Journal(JournalLoader.Current);

        public void OnEnable() { _savedGamePath = PathUtil.Combine(Application.persistentDataPath, "save.dat"); }

        public void Load()
        {
            if (!File.Exists(_savedGamePath))
            {
                Debug.Log("Unable to find game save -- creating new one");
                Data = new GameSaveData();
                foreach (var journal in JournalLoader.Journals)
                {
                    Data.Journal(journal);
                }
                Save();
            }
            else
            {
                Data = _serializer.Deserialize<GameSaveData>(_savedGamePath);
            }
        }

        public void Save() { _serializer.Serialize(Data, _savedGamePath); }
    }
}
