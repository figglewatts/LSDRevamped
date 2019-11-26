using System.Collections.Generic;
using System.IO;
using LSDR.Dream;
using LSDR.Util;
using Torii.Serialization;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName="System/JournalLoaderSystem")]
    public class JournalLoaderSystem : ScriptableObject
    {
        public string PathToJournals;
        public List<DreamJournal> Journals;

        public DreamJournal CurrentJournal => Journals[_currentJournalHandle];

        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private int _currentJournalHandle = 0;
        
        public void LoadJournals()
        {
            var journalsPath = IOUtil.PathCombine(Application.streamingAssetsPath, PathToJournals);
            var journalFiles = Directory.GetFiles(journalsPath, "*.json");
            Journals = new List<DreamJournal>();
            foreach (var journalFile in journalFiles)
            {
                var journal = _serializer.Deserialize<DreamJournal>(journalFile);
                Journals.Add(journal);
            }
        }

        public void SelectJournal(int idx)
        {
            if (idx < 0 || idx >= Journals.Count)
            {
                Debug.LogError($"Cannot select journal {idx}, as it's outside the bounds of the currently loaded journals");
                return;
            }

            _currentJournalHandle = idx;
        }
    }
}
