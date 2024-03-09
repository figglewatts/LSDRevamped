using System.Collections.Generic;
using LSDR.Game;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;

namespace LSDR.Lua.Persistence
{
    public class Persistence
    {
        protected GameSaveSystem _gameSaveSystem;

        protected Dictionary<string, PersistenceRecord> _dataStore = new();

        protected record PersistenceRecord(object Value, Lifetime Lifetime);

        public Persistence(GameSaveSystem gameSaveSystem)
        {
            _gameSaveSystem = gameSaveSystem;
        }

        public void SerializeToSaveData(GameSaveData.JournalSaveData saveData)
        {
            foreach (var (key, val) in _dataStore)
            {
                if (val.Lifetime != Lifetime.SaveFile) continue;
                saveData.LuaPersisted[key] = val.Value;
            }
        }

        public void DeserializeFromSaveData(GameSaveData.JournalSaveData saveData)
        {
            if (saveData.LuaPersisted == null) return;
            foreach (var (key, val) in saveData.LuaPersisted)
            {
                _dataStore[key] = new PersistenceRecord(val, Lifetime.SaveFile);
            }
        }

        public void Clear(Lifetime lifetime)
        {
            HashSet<string> toRemove = new();
            foreach (var key in _dataStore.Keys)
            {
                var record = _dataStore[key];
                if (record.Lifetime == lifetime) toRemove.Add(key);
            }
            foreach (var key in toRemove) _dataStore.Remove(key);
        }

        public void StoreNumber(string key, Lifetime lifetime, DynValue number)
        {
            _dataStore[key] = new PersistenceRecord(number.Number, lifetime);
        }

        public void StoreString(string key, Lifetime lifetime, string str)
        {
            _dataStore[key] = new PersistenceRecord(str, lifetime);
        }

        public void StoreBoolean(string key, Lifetime lifetime, bool b)
        {
            _dataStore[key] = new PersistenceRecord(b, lifetime);
        }

        public object RetrieveString(string key)
        {
            return _dataStore[key].Value as string;
        }

        public DynValue RetrieveNumber(string key)
        {
            return DynValue.NewNumber((double)_dataStore[key].Value);
        }

        public bool RetrieveBoolean(string key)
        {
            return (bool)_dataStore[key].Value;
        }

        public bool HasValue(string key)
        {
            return _dataStore.ContainsKey(key);
        }

        public void RemoveValue(string key)
        {
            _dataStore.Remove(key);
        }
    }
}
