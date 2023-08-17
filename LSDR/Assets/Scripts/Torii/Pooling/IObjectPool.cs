using UnityEngine;

namespace Torii.Pooling
{
    public interface IObjectPool
    {
        int Active { get; }

        GameObject PoolObject { get; }

        GameObject Summon(Vector3 pos, Quaternion rot, Transform parent = null);

        void Return(PoolItem item);

        void ReturnAll();

        void ActivePoolItemDestroyed(PoolItem item);
    }
}
