using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;

namespace RGSMS.Scene
{
    [CustomPropertyDrawer(typeof(SceneInspector))]
    public sealed class SceneInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string sceneName = string.Empty;
            GUIStyle normalStyle = new GUIStyle(EditorStyles.label);
            normalStyle.fontSize = 17;

            property.serializedObject.Update();

            SerializedProperty scenePath = property.FindPropertyRelative("_path");
            SerializedProperty sceneBuildIndex = property.FindPropertyRelative("_buildIndex");

            EditorGUI.DrawRect(position, new Color(0.1f, 0.1f, 0.1f, 1.0f));

            #region Title

            Rect titleRect = position;
            titleRect.x += 5.0f;
            titleRect.y -= 32.5f;

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = 20;

            EditorGUI.LabelField(titleRect, "Scene Infos:", style);

            Rect rect = position;
            rect.x += 5.0f;
            rect.y += 25.0f;
            rect.width -= 10.0f;
            rect.height = 18.0f;

            #endregion

            #region Select Scene Field

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath.stringValue);

            if(sceneAsset == null)
            {
                if(sceneBuildIndex.intValue != -1)
                {
                    string currentScenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex.intValue);

                    if (string.Compare(GetSceneName(currentScenePath), GetSceneName(scenePath.stringValue)) == 0)
                    {
                        scenePath.stringValue = currentScenePath;
                        sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath.stringValue);
                    }
                }
            }

            sceneAsset = (SceneAsset)EditorGUI.ObjectField(rect, "Scene:", sceneAsset, typeof(SceneAsset), false);

            if(sceneAsset != null)
            {
                string sceneCompletePath = AssetDatabase.GetAssetPath(sceneAsset);
                
                string path = Application.dataPath;
                for (int i = path.Length - 1; i >= 0; i--)
                {
                    if (path[i] == '/')
                    {
                        path = path.Remove(i + 1);

                        break;
                    }
                }

                scenePath.stringValue = sceneCompletePath.Replace(path, string.Empty);
            }

            if (!string.IsNullOrEmpty(scenePath.stringValue))
            {
                sceneName = GetSceneName(scenePath.stringValue);

                int buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath.stringValue);

                if (sceneBuildIndex.intValue != buildIndex)
                {
                    sceneBuildIndex.intValue = buildIndex;
                }
            }

            #endregion

            #region Scene Name

            style.fontSize = 15;

            Rect labelSceneNameArea = position;
            labelSceneNameArea.x += 5.0f;
            labelSceneNameArea.y += 10.0f;

            EditorGUI.LabelField(labelSceneNameArea, "Name:", style);

            labelSceneNameArea.x += 90.0f;

            if (sceneBuildIndex.intValue == -1)
            {
                GUI.color = Color.red;
            }

            EditorGUI.LabelField(labelSceneNameArea, sceneName, normalStyle);
            GUI.color = Color.white;

            #endregion

            #region Scene Build Index

            Rect labelBuildIndexArea = position;
            labelBuildIndexArea.x += 5.0f;
            labelBuildIndexArea.y += 30.0f;

            EditorGUI.LabelField(labelBuildIndexArea, "Build Index:", style);

            labelBuildIndexArea.x += 90.0f;

            string indexLabel;
            
            if (sceneBuildIndex.intValue != -1)
            {
                indexLabel = sceneBuildIndex.intValue.ToString();
            }
            else
            {
                if(string.IsNullOrEmpty(scenePath.stringValue))
                {
                    GUI.color = Color.yellow;
                    indexLabel = "Please, select a scene!";
                }
                else
                {
                    GUI.color = Color.red;
                    indexLabel = $"Please, add the scene {sceneName} to Build Settings!";
                }
            }

            EditorGUI.LabelField(labelBuildIndexArea, indexLabel, normalStyle);

            #endregion

            property.serializedObject.ApplyModifiedProperties();
        }

        private string GetSceneName (string ScenePath)
        {
            string path = ScenePath;
            string name = string.Empty;

            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '/')
                {
                    name = path.Remove(0, i + 1);

                    break;
                }
            }

            return name.Replace(".unity", string.Empty);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, false) * 5.0f;
        }
    }
}
