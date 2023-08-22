using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Util
{
    [Serializable]
    public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [HideInInspector] [SerializeField] protected List<K> _keys = new List<K>();
        [HideInInspector] [SerializeField] protected List<V> _values = new List<V>();

        #if UNITY_EDITOR
        // used for add functionality in CustomPropertyDrawer
        [HideInInspector] [SerializeField] protected K _tempKey;
        [HideInInspector] [SerializeField] protected V _tempValue;
        #endif

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in this)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < Math.Min(_keys.Count, _values.Count); i++)
            {
                Add(_keys[i], _values[i]);
            }
        }
    }
}
