using System.Collections.Generic;
using System.Linq;
using LSDR.Dream;
using LSDR.SDK.Data;
using Newtonsoft.Json;

namespace LSDR.Game
{
    [JsonObject]
    public class GameSaveData
    {
        public readonly Dictionary<string, JournalSaveData> JournalSaves;

        public GameSaveData() { JournalSaves = new Dictionary<string, JournalSaveData>(); }

        public JournalSaveData Journal(DreamJournal journal)
        {
            if (!JournalSaves.ContainsKey(journal.Name)) JournalSaves[journal.Name] = new JournalSaveData();

            return JournalSaves[journal.Name];
        }

        [JsonObject]
        public class JournalSaveData
        {
            public readonly List<DreamSequence> SequenceData = new List<DreamSequence>();

            [JsonIgnore]
            public int DayNumber => SequenceData.Count + 1;

            [JsonIgnore]
            public int LastGraphX => SequenceData.Last().DynamicScore + 9;

            [JsonIgnore]
            public int LastGraphY => SequenceData.Last().UpperScore + 9;
        }
    }
}
