using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
#endif

namespace LSDR.SDK.Util
{
    [Serializable]
    public class SceneProperty
    {
        [SerializeField] private Object sceneAsset;
        [SerializeField] private string scenePath;

        public static implicit operator string(SceneProperty property)
        {
            return property.scenePath;
        }
    }

    #if UNITY_EDITOR

    #endif
}
