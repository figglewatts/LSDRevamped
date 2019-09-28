using UnityEngine;

namespace Torii.Pooling
{
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
    }
}