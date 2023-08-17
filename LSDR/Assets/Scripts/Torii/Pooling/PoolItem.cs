using UnityEngine;

namespace Torii.Pooling
{
    [ExecuteInEditMode] // to handle OnDestroy callbacks in editor
    public class PoolItem : MonoBehaviour
    {
        public IObjectPool ParentPool { get; set; }

        public bool InPool { get; set; } = true;

        private void OnDestroy()
        {
            // if we're active, then we need to make sure an inactive object still exists in the pool,
            // otherwise it might try to instantiate a deleted object in future
            if (!InPool && ParentPool.PoolObject != null &&
                !ParentPool.PoolObject.GetComponent<PoolObject>().IsDestroyed)
            {
                //Debug.Log("ITEM DESTROYED");
                //ParentPool.ActivePoolItemDestroyed(this);
            }
        }

        public void ActiveState(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Return()
        {
            ParentPool.Return(this);
        }
    }
}
