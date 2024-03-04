using System;
using System.Collections.Generic;
using System.Linq;
using LSDR.Dream;
using LSDR.SDK.Data;
using Newtonsoft.Json;
using UnityEngine;
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
            if (!JournalSaves.ContainsKey(journal.Name))
            {
                JournalSaves[journal.Name] = new JournalSaveData();
                JournalSaves[journal.Name].SetDayNumber(1);
            }

            return JournalSaves[journal.Name];
        }

        [JsonObject]
        public class JournalSaveData
        {
            [JsonProperty("SequenceData")]
            protected readonly List<DreamSequence> _sequenceData = new List<DreamSequence>();

            public int DayNumber { get; set; }

            public int YearNumber { get; set; }

            [JsonIgnore] public IEnumerable<DreamSequence> SequenceData => _sequenceData;

            [JsonIgnore]
            public int LastGraphX
            {
                get
                {
                    if (_sequenceData.Count == 0) return 0;
                    return _sequenceData.Last().EvaluateGraphPosition().x;
                }
            }

            [JsonIgnore]
            public int LastGraphY
            {
                get
                {
                    if (_sequenceData.Count == 0) return 0;
                    return _sequenceData.Last().EvaluateGraphPosition().y;
                }
            }

            [JsonIgnore]
            public int NumberOfSequences => _sequenceData.Count;

            [JsonIgnore]
            public Action OnDayNumberChanged;

            public bool HasEnoughDataForFlashback()
            {
                return SequenceData.SelectMany(sd => sd.EntityGraphContributions).Count() > 100;
            }

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
                if (newDayNumber <= 0)
                {
                    Debug.LogError("Can't set day to number less than 1");
                    return;
                }

                DayNumber = (newDayNumber - 1) % 365 + 1;
                YearNumber = (newDayNumber - 1) / 365;
                OnDayNumberChanged?.Invoke();
            }

            public DreamSequence GetSequence(int index)
            {
                return _sequenceData[index];
            }
        }
    }
}
