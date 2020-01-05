using System.Collections.Generic;
using LSDR.Dream;
using ProtoBuf;

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
            public List<DreamSequence> SequenceData = new List<DreamSequence>();

            [ProtoIgnore]
            public int DayNumber => SequenceData.Count + 1;
        }
    }
}
