using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    public abstract class BaseFootstepIndex<T> : AbstractFootstepIndex
    {
        public SerializableDictionary<T, Footstep> Index;
    }
}
