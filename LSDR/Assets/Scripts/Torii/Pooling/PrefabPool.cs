using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.Pooling
{
    public class PrefabPool : MonoBehaviour, IObjectPool
    {
        public PoolItem Prefab;

        public int Size;

        public bool DestroyOnLoad;

        public int Active => _activeItems.Count;

        private Stack<PoolItem> _pool;
        private List<PoolItem> _activeItems;
        
        private bool _populated = false;

        public static GameObject Create(string poolName, PoolItem prefab, int size, bool destroyOnLoad = false)
        {
            GameObject pool = new GameObject(poolName);
            var poolScript = pool.AddComponent<PrefabPool>();
            poolScript.Prefab = prefab;
            poolScript.Size = size;
            poolScript.DestroyOnLoad = destroyOnLoad;
            poolScript.populate();
            return pool;
        }

        public void Awake()
        {
            _pool = new Stack<PoolItem>(Size);
            _activeItems = new List<PoolItem>();

            if (!DestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }

            if (Size > 0)
            {
                populate();
            }
        }

        public GameObject Summon(Vector3 pos, Quaternion rot, Transform parent = null)
        {
            PoolItem item = _pool.Pop();
            _activeItems.Add(item);
            if (parent != null)
            {
                item.transform.SetParent(parent);
            }

            item.transform.SetPositionAndRotation(pos, rot);
            item.ActiveState(true);
            return item.gameObject;
        }

        public void Return(PoolItem item)
        {
            item.ActiveState(false);
            _activeItems.Remove(item);
            _pool.Push(item);
            item.transform.SetParent(transform);
        }

        private PoolItem create(bool activeState = false)
        {
            PoolItem obj = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            obj.ParentPool = this;
            obj.ActiveState(activeState);
            obj.transform.SetParent(this.transform, false);
            return obj;
        }

        private void populate()
        {
            if (_populated)
            {
                Debug.LogWarning("Attempting to populate an already populated PrefabPool!");
                return;
            }
            
            for (int i = 0; i < Size; i++)
            {
                PoolItem obj = create();
                _pool.Push(obj);
            }

            _populated = true;
        }
    }
}