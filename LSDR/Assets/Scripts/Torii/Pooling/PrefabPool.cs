using System.Collections;
using System.Collections.Generic;
using Torii.Coroutine;
using UnityEngine;

namespace Torii.Pooling
{
    [CreateAssetMenu(menuName = "Torii/PrefabPool")]
    public class PrefabPool : ScriptableObject, IObjectPool
    {
        public PoolItem Prefab;

        public int Size;

        public bool Persistent;

        public string Name;
        private List<PoolItem> _activeItems;

        private Stack<PoolItem> _pool;

        public GameObject PoolObject { get; private set; }

        public int Active => _activeItems.Count;

        public GameObject Summon(Vector3 pos, Quaternion rot, Transform parent = null)
        {
            PoolItem item = _pool.Pop();
            _activeItems.Add(item);
            if (parent != null)
            {
                item.transform.SetParent(parent);
            }

            item.transform.SetPositionAndRotation(pos, rot);
            item.ActiveState(state: true);
            item.InPool = false;
            return item.gameObject;
        }

        public void Return(PoolItem item)
        {
            item.ActiveState(state: false);
            _activeItems.Remove(item);
            _pool.Push(item);
            item.InPool = true;
            item.transform.SetParent(PoolObject.transform);
        }

        public void ReturnAll()
        {
            for (int i = _activeItems.Count - 1; i >= 0; i--)
            {
                Return(_activeItems[i]);
            }
        }

        public void ActivePoolItemDestroyed(PoolItem item)
        {
            // remove the item from the list of active items
            _activeItems.Remove(item);

            // make sure we create a new one in the pool to replace
            // the deleted one
            _pool.Push(create());
        }

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

        public IEnumerator ReturnAllCoroutine() { yield return returnAllAsync(); }

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

        private PoolItem create(bool activeState = false)
        {
            PoolItem obj = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            obj.ParentPool = this;
            obj.InPool = true;
            obj.ActiveState(activeState);
            obj.transform.SetParent(PoolObject.transform, worldPositionStays: false);
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
            using (Marathon m = new Marathon(timeThresholdMs: 10))
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

        private IEnumerator returnAllAsync()
        {
            using (Marathon m = new Marathon(timeThresholdMs: 10))
            {
                bool lastYield = false;
                for (int i = _activeItems.Count - 1; i >= 0; i--)
                {
                    int i1 = i;
                    lastYield = m.Run(() =>
                    {
                        Return(_activeItems[i1]);
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
