using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Torii.Pooling
{
    public interface IObjectPool
    {
        int Active { get; }
        
        GameObject Summon(Vector3 pos, Quaternion rot, Transform parent = null);

        void Return(PoolItem item);
    }
}