using System;
using System.Collections;
using System.Collections.Generic;
using Torii.Coroutine;
using UnityEditor;
using UnityEngine;

namespace Torii.Pooling
{
    [CreateAssetMenu(menuName="Torii/PrefabPool")]
    public class PrefabPool : ScriptableObject, IObjectPool
    {
        public PoolItem Prefab;

        public int Size;

        public bool Persistent;

        public string Name;

        [NonSerialized]
        public GameObject PoolObject;
        
        public int Active => _activeItems.Count;

        private Stack<PoolItem> _pool;
        private List<PoolItem> _activeItems;
        
        [NonSerialized]
        private bool _populated = false;

        public void Initialise()
        {
            commonInitialise();
            populate();
        }

        public IEnumerator InitialiseCoroutine()
        {
            commonInitialise();
            yield return populateAsync();
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
            item.transform.SetParent(PoolObject.transform);
        }

        private void commonInitialise()
        {
            PoolObject = new GameObject(Name);
            if (Persistent)
            {
                DontDestroyOnLoad(PoolObject);
            }
            _pool = new Stack<PoolItem>(Size);
            _activeItems = new List<PoolItem>();
        }

        private PoolItem create(bool activeState = false)
        {
            PoolItem obj = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            obj.ParentPool = this;
            obj.ActiveState(activeState);
            obj.transform.SetParent(PoolObject.transform, false);
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

        private IEnumerator populateAsync()
        {
            if (_populated)
            {
                Debug.LogWarning("Attempting to populate an already populated PrefabPool!");
                yield break;
            }
            
            using (Marathon m = new Marathon(10))
            {
                bool lastYield = false;
                for (int i = 0; i < Size; i++)
                {
                    lastYield = m.Run(() =>
                    {
                        PoolItem obj = create();
                        _pool.Push(obj);
                    }, lastYield);

                    if (lastYield)
                    {
                        yield return null;
                    }
                }
            }

            _populated = true;
        }
    }
}