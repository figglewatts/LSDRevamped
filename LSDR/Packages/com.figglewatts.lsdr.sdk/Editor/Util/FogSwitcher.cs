using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Util
{
    public class FogSwitcher
    {
        private static readonly int SubtractiveFog = Shader.PropertyToID("_SubtractiveFog");

        [MenuItem("LSDR SDK/Toggle fog kind")]
        public static void SwitchFogKind()
        {
            var existing = Shader.GetGlobalInt(SubtractiveFog);
            var newFogKind = existing == 1 ? 0 : 1;
            Shader.SetGlobalInt(SubtractiveFog, newFogKind);
            var fogKindString = newFogKind == 1 ? "Subtractive" : "Additive";
            Debug.Log($"Set fog kind to: '{fogKindString}'");
        }
    }
}
