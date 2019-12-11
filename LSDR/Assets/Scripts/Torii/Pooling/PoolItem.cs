using System;
using UnityEngine;

namespace Torii.Pooling
{
    [ExecuteInEditMode] // to handle OnDestroy callbacks in editor
    public class PoolItem : MonoBehaviour
    {
        public IObjectPool ParentPool { get; set; }
        
        public void ActiveState(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Return()
        {
            ParentPool.Return(this);
        }

        private void OnDestroy()
        {
            // if we're active, then we need to make sure an inactive object still exists in the pool,
            // otherwise it might try to instantiate a deleted object in future
            if (ParentPool.PoolObject != null && !ParentPool.PoolObject.GetComponent<PoolObject>().IsDestroyed)
            {
                ParentPool.ActivePoolItemDestroyed(this);
            }
        }
    }
}