using UnityEngine;
using UnityEditor;

namespace RGSMS
{
    [CustomPropertyDrawer(typeof(SceneFinderAttribute))]
    public class SceneFinderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            Rect rect = position;
            rect.height = 20.0f;

            GUI.enabled = false;
            EditorGUI.PropertyField(rect, property, label);
            GUI.enabled = true;

            rect.y += 22.0f;
            rect.x += 50.0f;
            rect.width -= 50.0f;

            SceneAsset sceneAsset = null;
            sceneAsset = (SceneAsset)EditorGUI.ObjectField(rect, string.Empty, sceneAsset, typeof(SceneAsset), false);

            if (sceneAsset != null)
            {
                property.stringValue = sceneAsset.name;
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property) + 28.0f;
        }
    }
}
