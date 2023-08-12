using LSDR.SDK.Assets;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [CustomEditor(typeof(TIMAsset))]
    public class TIMEditor : UnityEditor.Editor
    {
        public override bool HasPreviewGUI() { return true; }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawPreviewTexture(r, ((TIMAsset)serializedObject.targetObject).Palettes[0], mat: null,
                    ScaleMode.ScaleToFit);
            }
        }
    }
}
