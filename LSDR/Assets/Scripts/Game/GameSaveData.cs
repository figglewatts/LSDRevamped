using System.Collections.Generic;
using System.Linq;
using LSDR.Dream;
using ProtoBuf;
using DreamJournal = LSDR.SDK.Data.DreamJournal;

namespace LSDR.Game
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class GameSaveData
    {
        public readonly Dictionary<string, JournalSaveData> JournalSaves;

        public GameSaveData()
        {
            JournalSaves = new Dictionary<string, JournalSaveData>();
        }

        public JournalSaveData Journal(DreamJournal journal)
        {
            if (!JournalSaves.ContainsKey(journal.Name))
            {
                JournalSaves[journal.Name] = new JournalSaveData();
            }

            return JournalSaves[journal.Name];
        }
        
        [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
        public class JournalSaveData
        {
            public readonly List<DreamSequence> SequenceData = new List<DreamSequence>();

            [ProtoIgnore]
            public int DayNumber => SequenceData.Count + 1;

            [ProtoIgnore]
            public int LastGraphX => SequenceData.Last().DynamicScore + 9;

            [ProtoIgnore]
            public int LastGraphY => SequenceData.Last().UpperScore + 9;
        }
    }
}
