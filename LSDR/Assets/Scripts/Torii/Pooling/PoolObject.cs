using System;
using UnityEngine;

namespace Torii.Pooling
{
    [ExecuteInEditMode] // to handle OnDestroy callbacks in editor
    public class PoolObject : MonoBehaviour
    {
        public IObjectPool ParentPool;
        public bool IsDestroyed = false;

        public void OnDestroy()
        {
            IsDestroyed = true;
        }
    }
}
