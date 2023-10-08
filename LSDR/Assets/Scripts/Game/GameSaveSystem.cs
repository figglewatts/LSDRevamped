using System.IO;
using LSDR.SDK.Data;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName = "System/GameSaveSystem")]
    public class GameSaveSystem : ScriptableObject
    {
        public SettingsSystem SettingsSystem;

        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        public GameSaveData Data { get; private set; }

        private string _savedGamePath
        {
            get
            {
                string saveDataFileName = PathUtil.SanitiseFileName($"{SettingsSystem.CurrentMod.Name}_save.json");
                return PathUtil.Combine(_saveDataDirectory, saveDataFileName);
            }
        }

        private string _saveDataDirectory => PathUtil.Combine(Application.persistentDataPath, "saves");

        public GameSaveData.JournalSaveData CurrentJournalSave => Data?.Journal(SettingsSystem.CurrentJournal);

        public void OnEnable() { Directory.CreateDirectory(_saveDataDirectory); }

        public void Load()
        {
            if (!File.Exists(_savedGamePath))
            {
                Debug.Log("Unable to find game save -- creating new one");
                Data = new GameSaveData();
                foreach (DreamJournal journal in SettingsSystem.CurrentMod.Journals) Data.Journal(journal);

                Save();
            }
            else
                Data = _serializer.Deserialize<GameSaveData>(_savedGamePath);
        }

        public void Save() { _serializer.Serialize(Data, _savedGamePath); }
    }
}
