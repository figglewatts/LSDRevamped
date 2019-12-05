using System;
using System.Collections;
using System.Collections.Generic;
using Torii.Coroutine;
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
        
        public GameObject PoolObject { get; private set; }

        public int Active => _activeItems.Count;
        
        private Stack<PoolItem> _pool;
        private List<PoolItem> _activeItems;

        public void Initialise()
        {
            CommonInitialise();
            populate();
        }

        public IEnumerator InitialiseCoroutine()
        {
            CommonInitialise();
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

        public void ReturnAll()
        {
            foreach (var item in _activeItems)
            {
                Return(item);
            }
        }
        
        public void CommonInitialise()
        {
            PoolObject = new GameObject(Name);
            PoolObject poolObjectScript = PoolObject.AddComponent<PoolObject>();
            poolObjectScript.ParentPool = this;
            if (Persistent)
            {
                DontDestroyOnLoad(PoolObject);
            }
            _pool = new Stack<PoolItem>(Size);
            _activeItems = new List<PoolItem>();
        }

        public void ActivePoolItemDestroyed(PoolItem item)
        {
            // remove the item from the list of active items
            _activeItems.Remove(item);
            
            // make sure we create a new one in the pool to replace
            // the deleted one
            _pool.Push(create());
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
            for (int i = 0; i < Size; i++)
            {
                PoolItem obj = create();
                _pool.Push(obj);
            }
        }

        private IEnumerator populateAsync()
        {
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
        }
    }
}