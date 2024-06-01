using System;
using System.Collections.Generic;
using System.Linq;
using LSDR.Dream;
using LSDR.Lua;
using LSDR.Lua.Persistence;
using LSDR.SDK.Data;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
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

        public void Initialise()
        {
            ((LuaEngine)LuaManager.Managed).Persistence.Clear(Lifetime.Mod);
            foreach (var journal in JournalSaves.Values)
            {
                journal.DeserializeLuaData();
            }
        }

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

            public readonly Dictionary<string, object> LuaPersisted = new();

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

            public void SerializeLuaData()
            {
                ((LuaEngine)LuaManager.Managed).Persistence.SerializeToSaveData(this);
            }

            public void DeserializeLuaData()
            {
                ((LuaEngine)LuaManager.Managed).Persistence.DeserializeFromSaveData(this);
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

            public void ResetProgress()
            {
                _sequenceData.Clear();
                ((LuaEngine)LuaManager.Managed).Persistence.ClearAll();
                LuaPersisted.Clear();
                DayNumber = 1;
                YearNumber = 0;
            }
        }
    }
}
