using LSDR.SDK.Data;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Data
{
    [CustomPropertyDrawer(typeof(GraphContribution))]
    public class GraphContributionPropertyDrawer : PropertyDrawer
    {
        protected bool _foldout = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty dynamicnessProperty = property.FindPropertyRelative("_vector.x");
            SerializedProperty uppernessProperty = property.FindPropertyRelative("_vector.y");

            position.height = 20f;
            _foldout = EditorGUI.Foldout(position, _foldout, label, toggleOnLabelClick: true);

            if (_foldout)
            {
                position.y += 20f;

                EditorGUI.indentLevel++;
                if (dynamicnessProperty != null && uppernessProperty != null)
                {
                    Rect dynamicnessRect = EditorGUI.IndentedRect(position);
                    Rect dynamicnessLabelRect = new Rect(dynamicnessRect.x, dynamicnessRect.y,
                        dynamicnessRect.width / 4,
                        dynamicnessRect.height);
                    Rect dynamicnessSliderRect = new Rect(dynamicnessRect.x + dynamicnessRect.width / 4,
                        dynamicnessRect.y,
                        3 * dynamicnessRect.width / 4, dynamicnessRect.height);
                    position.y += 20f;
                    Rect uppernessRect = EditorGUI.IndentedRect(position);
                    Rect uppernessLabelRect = new Rect(uppernessRect.x, uppernessRect.y, uppernessRect.width / 4,
                        uppernessRect.height);
                    Rect uppernessSliderRect = new Rect(uppernessRect.x + uppernessRect.width / 4, uppernessRect.y,
                        3 * uppernessRect.width / 4, uppernessRect.height);

                    EditorGUI.LabelField(dynamicnessLabelRect, "Dynamic");
                    EditorGUI.IntSlider(dynamicnessSliderRect, dynamicnessProperty, -9, 9, GUIContent.none);
                    EditorGUI.LabelField(uppernessLabelRect, "Upper");
                    EditorGUI.IntSlider(uppernessSliderRect, uppernessProperty, -9, 9, GUIContent.none);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _foldout ? 20f * 3 : 20f;
        }
    }
}
