using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LSDR.SDK.Util
{
    [Serializable]
    public class SceneProperty
    {
        [SerializeField] private Object sceneAsset;
        [SerializeField] private string scenePath;

        public static implicit operator string(SceneProperty property) => property.scenePath;
    }

#if UNITY_EDITOR
    
#endif
}
