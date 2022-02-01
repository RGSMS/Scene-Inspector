using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace RGSMS.Scene
{
    public enum ESceneEditorStatus
    {
        Empty = 0,
        LostScene,
        NeedToAddToBuild,
        WorkingWell
    }

    [CustomPropertyDrawer(typeof(SceneInspector))]
    public sealed class SceneInspectorDrawer : PropertyDrawer
    {
        private const string _texturePath = "Assets/Editor/SceneInspector/Textures/signal.png";

        private readonly GUIStyle _boldStyle = new GUIStyle(EditorStyles.boldLabel);

        private readonly Color _darkGrey = new Color(0.16f, 0.16f, 0.16f, 1.0f);
        private readonly Color _grey = new Color(0.25f, 0.25f, 0.25f, 1.0f);

        private readonly Dictionary<ESceneEditorStatus, Dictionary<Color, string>> _statusDic = new Dictionary<ESceneEditorStatus, Dictionary<Color, string>>()
        {
            { ESceneEditorStatus.Empty,             new Dictionary<Color, string>() { [Color.blue]     = "Select a scene" } },
            { ESceneEditorStatus.LostScene,         new Dictionary<Color, string>() { [Color.red]       = "The scene {0} has been deleted or moved" } },
            { ESceneEditorStatus.NeedToAddToBuild,  new Dictionary<Color, string>() { [Color.yellow]    = "Add scene to the Build Settings" } },
            { ESceneEditorStatus.WorkingWell,       new Dictionary<Color, string>() { [Color.green]      = "Everything is alright" } }
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            Rect foldoutRect = position;
            foldoutRect.height = 18.0f;

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
            if (property.isExpanded)
            {
                ESceneEditorStatus status = ESceneEditorStatus.Empty;

                SerializedProperty scenePath = property.FindPropertyRelative("_path");
                SerializedProperty sceneBuildIndex = property.FindPropertyRelative("_buildIndex");

                DrawBackground(position);

                DrawSceneField(position, scenePath, sceneBuildIndex, ref status);
                DrawSceneInfos(position, scenePath, sceneBuildIndex.intValue, status);
                DrawSceneStatus(position, scenePath, status);
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            if(property.isExpanded)
            {
                return (EditorGUI.GetPropertyHeight(property, label, false) * 5.0f) + 10.0f;
            }

            return EditorGUI.GetPropertyHeight(property, label, false);
        }

        private void DrawSceneField (Rect position, SerializedProperty scenePath, SerializedProperty sceneBuildIndex, ref ESceneEditorStatus status)
        {
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath.stringValue);

            if (sceneAsset == null)
            {
                if (sceneBuildIndex.intValue != -1)
                {
                    string currentScenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex.intValue);

                    if (string.Compare(GetSceneName(currentScenePath), GetSceneName(scenePath.stringValue)) == 0)
                    {
                        scenePath.stringValue = currentScenePath;
                        sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath.stringValue);

                        status = ESceneEditorStatus.WorkingWell;
                    }
                    else
                    {
                        status = ESceneEditorStatus.LostScene;
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(scenePath.stringValue))
                    {
                        status = ESceneEditorStatus.LostScene;
                    }
                }
            }
            else
            {
                status = ESceneEditorStatus.WorkingWell;
            }

            Rect rect = position;
            rect.x += 10.5f;
            rect.y += 25.0f;
            rect.width = 40.0f;
            rect.height = 18.0f;

            EditorGUI.LabelField(rect, "Scene:", _boldStyle);

            rect.x += 45.0f;
            rect.width = position.width;
            rect.width -= 62.0f;

            sceneAsset = (SceneAsset)EditorGUI.ObjectField(rect, string.Empty, sceneAsset, typeof(SceneAsset), false);

            if (sceneAsset == null)
            {
                if (status == ESceneEditorStatus.LostScene)
                {
                    return;
                }
                else if(status == ESceneEditorStatus.WorkingWell)
                {
                    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                    EditorBuildSettingsScene[] filteredScenes = scenes.Where(scene => File.Exists(scene.path)).ToArray();
                    EditorBuildSettings.scenes = filteredScenes;

                    string currentScenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex.intValue);

                    if (string.Compare(GetSceneName(currentScenePath), GetSceneName(scenePath.stringValue)) != 0)
                    {
                        sceneBuildIndex.intValue = -1;
                        scenePath.stringValue = string.Empty;
                        status = ESceneEditorStatus.LostScene;
                        return;
                    }

                    status = ESceneEditorStatus.Empty;
                    scenePath.stringValue = string.Empty;
                    sceneBuildIndex.intValue = -1;
                    return;
                }
            }
            else
            {
                string sceneCompletePath = AssetDatabase.GetAssetPath(sceneAsset);

                int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneCompletePath);

                if (buildIndex == -1)
                {
                    sceneBuildIndex.intValue = -1;
                    scenePath.stringValue = sceneCompletePath;
                    status = ESceneEditorStatus.NeedToAddToBuild;
                    return;
                }
                else
                {
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
                    
                    buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath.stringValue);

                    if (sceneBuildIndex.intValue != buildIndex)
                    {
                        sceneBuildIndex.intValue = buildIndex;

                        if (sceneBuildIndex.intValue != -1)
                        {
                            status = ESceneEditorStatus.NeedToAddToBuild;
                        }
                    }
                }
            }
        }

        private void DrawSceneInfos (Rect position, SerializedProperty scenePath, int buildIndex, ESceneEditorStatus eStatus)
        {
            DrawLine(position, 47.5f);

            Rect labelInfosArea = position;
            labelInfosArea.x += 10.5f;
            labelInfosArea.y += 10.0f;

            EditorGUI.LabelField(labelInfosArea, "Build Index:", _boldStyle);

            labelInfosArea.x += 70.5f;

            EditorGUI.LabelField(labelInfosArea, buildIndex.ToString("00"));

            if (eStatus != ESceneEditorStatus.Empty &&
                eStatus != ESceneEditorStatus.LostScene)
            {
                labelInfosArea.x += 30.0f;
                labelInfosArea.width += 8.0f;
                labelInfosArea.width -= labelInfosArea.x;
                labelInfosArea.height = 20.0f;
                labelInfosArea.y += 41.0f;

                scenePath.isExpanded = EditorGUI.ToggleLeft(labelInfosArea, scenePath.isExpanded ? scenePath.stringValue : "Show Path!", scenePath.isExpanded);
            }
        }

        private void DrawSceneStatus (Rect position, SerializedProperty scenePath, ESceneEditorStatus eStatus)
        {
            Color color = Color.white;
            string text = string.Empty;

            foreach(KeyValuePair<Color, string> status in _statusDic[eStatus])
            {
                color = status.Key;
                text = status.Value;
            }

            DrawLine(position, 72.5f);

            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(_texturePath);

            Rect statusRect = position;
            statusRect.x += 2.0f;
            statusRect.y += 71.5f;

            statusRect.width = texture.width;
            statusRect.height = texture.height;

            GUI.color = color;
            GUI.DrawTexture(statusRect, texture);
            GUI.color = Color.white;

            statusRect.x += 27.5f;

            if (eStatus == ESceneEditorStatus.NeedToAddToBuild)
            {
                statusRect.y += 7.5f;

                statusRect.height = 20.0f;

                statusRect.width = position.width;
                statusRect.width -= statusRect.x;
                statusRect.width += 10.0f;

                if (GUI.Button(statusRect, text))
                {
                    List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

                    scenes.AddRange(EditorBuildSettings.scenes);
                    scenes.Add(new EditorBuildSettingsScene(scenePath.stringValue, true));

                    EditorBuildSettings.scenes = scenes.ToArray();
                }
            }
            else
            {
                statusRect.y -= 33.5f;

                statusRect.height = position.height;

                statusRect.width = position.width;
                statusRect.width -= statusRect.x;
                statusRect.width += 10.0f;

                if (eStatus != ESceneEditorStatus.LostScene)
                {
                    EditorGUI.LabelField(statusRect, text, _boldStyle);
                }
                else
                {
                    EditorGUI.LabelField(statusRect, text.Replace("{0}", GetSceneName(scenePath.stringValue)), _boldStyle);
                }
            }
        }

        private void DrawLine (Rect position, float plusY)
        {
            Rect lineRect = position;
            lineRect.x += 10.5f;
            lineRect.y += plusY;
            lineRect.width -= 17.5f;
            lineRect.height = 2.0f;

            EditorGUI.DrawRect(lineRect, _grey);
        }

        private void DrawBackground (Rect position)
        {
            Rect backgroundRectLayerOne = position;
            backgroundRectLayerOne.x += 2.0f;
            backgroundRectLayerOne.y += 17.0f;
            backgroundRectLayerOne.width -= 2.0f;
            backgroundRectLayerOne.height -= 10.0f;

            Rect backgroundRectLayerTwo = position;
            backgroundRectLayerTwo.x += 4.0f;
            backgroundRectLayerTwo.y += 19.0f;
            backgroundRectLayerTwo.width -= 6.0f;
            backgroundRectLayerTwo.height -= 14.0f;

            Rect backgroundRectLayerThree = position;
            backgroundRectLayerThree.x += 6.0f;
            backgroundRectLayerThree.y += 20.0f;
            backgroundRectLayerThree.width -= 10.0f;
            backgroundRectLayerThree.height -= 16.0f;

            EditorGUI.DrawRect(backgroundRectLayerOne, _darkGrey);
            EditorGUI.DrawRect(backgroundRectLayerTwo, _grey);
            EditorGUI.DrawRect(backgroundRectLayerThree, _darkGrey);
        }

        private void UpdateScenePathAndIndex (SerializedProperty scenePath, SerializedProperty sceneBuildIndex, SceneAsset sceneAsset, ref ESceneEditorStatus status)
        {
            if (!string.IsNullOrEmpty(scenePath.stringValue))
            {
                int buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath.stringValue);

                if (sceneBuildIndex.intValue != buildIndex)
                {
                    sceneBuildIndex.intValue = buildIndex;

                    if (sceneBuildIndex.intValue != -1)
                    {
                        status = ESceneEditorStatus.NeedToAddToBuild;
                    }
                }
            }
            else
            {
                sceneBuildIndex.intValue = -1;
            }
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
    }
}
