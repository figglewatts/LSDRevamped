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
            int existing = Shader.GetGlobalInt(SubtractiveFog);
            int newFogKind = existing == 1 ? 0 : 1;
            Shader.SetGlobalInt(SubtractiveFog, newFogKind);
            string fogKindString = newFogKind == 1 ? "Subtractive" : "Additive";
            Debug.Log($"Set fog kind to: '{fogKindString}'");
        }
    }
}
