using System;
using System.Collections.Generic;
using System.Linq;
using LSDR.Dream;
using LSDR.SDK.Data;
using Newtonsoft.Json;
using UnityEngine.Assertions;

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
            [JsonProperty("SequenceData")]
            protected readonly List<DreamSequence> _sequenceData = new List<DreamSequence>();

            public int DayNumber { get; protected set; } = 1;

            public int YearNumber { get; protected set; } = 0;

            [JsonIgnore]
            public int LastGraphX => _sequenceData.Last().EvaluateGraphPosition().x;

            [JsonIgnore]
            public int LastGraphY => _sequenceData.Last().EvaluateGraphPosition().y;

            [JsonIgnore]
            public int NumberOfSequences => _sequenceData.Count;

            [JsonIgnore]
            public Action OnDayNumberChanged;

            public void IncrementDayNumberWithSequence(DreamSequence sequence)
            {
                sequence.SetDayNumber(DayNumber);
                _sequenceData.Add(sequence);

                DayNumber++;
                if (DayNumber > 365)
                {
                    DayNumber = 1;
                    YearNumber++;
                }
                OnDayNumberChanged?.Invoke();
            }

            public void SetDayNumber(int newDayNumber)
            {
                int yearNumber = newDayNumber / 365;
                DayNumber = newDayNumber % 365;
                YearNumber = yearNumber;
                OnDayNumberChanged?.Invoke();
            }

            public DreamSequence GetSequence(int index)
            {
                return _sequenceData[index];
            }
        }
    }
}
