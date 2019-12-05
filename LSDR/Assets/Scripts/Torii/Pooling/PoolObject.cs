using System;
using UnityEngine;

namespace Torii.Pooling
{
    [ExecuteInEditMode] // to handle OnDestroy callbacks in editor
    public class PoolObject : MonoBehaviour
    {
        public IObjectPool ParentPool;

        public void OnDestroy()
        {
            if (ParentPool.PoolObject != null)
            {
                ParentPool.ReturnAll();
            }
        }
    }
}
