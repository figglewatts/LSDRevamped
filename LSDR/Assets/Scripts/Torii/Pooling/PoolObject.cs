using UnityEngine;

namespace Torii.Pooling
{
    [ExecuteInEditMode] // to handle OnDestroy callbacks in editor
    public class PoolObject : MonoBehaviour
    {
        public bool IsDestroyed;
        public IObjectPool ParentPool;

        public void OnDestroy()
        {
            Debug.Log("POOL DESTROYED");
            IsDestroyed = true;
        }
    }
}
