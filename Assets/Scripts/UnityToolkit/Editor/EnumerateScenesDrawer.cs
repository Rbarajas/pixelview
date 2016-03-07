using UnityEditor;
using UnityEngine;

namespace UnityToolkit
{
    [CustomPropertyDrawer(typeof(EnumerateScenesAttribute))]
    public class EnumerateScenesDrawer : PropertyDrawer
    {
        private string[] m_SceneNames;

        private int m_SelectedIndex;
        

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_SceneNames == null && EditorBuildSettings.scenes.Length > 0)
            {
                m_SceneNames = new string[EditorBuildSettings.scenes.Length];

                for (var sceneIndex = 0; sceneIndex < m_SceneNames.Length; ++sceneIndex)
                {
                    var scenePath = EditorBuildSettings.scenes[sceneIndex].path;
                    var sceneName = scenePath.Substring(scenePath.LastIndexOf('/') + 1).Replace(".unity", string.Empty);

                    m_SceneNames[sceneIndex] = sceneName;

                    if (property.stringValue == sceneName)
                        m_SelectedIndex = sceneIndex;
                }
            }

            if (m_SceneNames != null)
            {
                m_SelectedIndex = EditorGUI.Popup(position, label.text, m_SelectedIndex, m_SceneNames);

                property.stringValue = m_SceneNames[m_SelectedIndex];
            }
            else
            {
                EditorGUI.LabelField(position, "No scenes added to Build Settings.");
            }
        }
    }
}
