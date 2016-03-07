using UnityEditor;

namespace UnityToolkit.MemoryManagement
{
    [CustomEditor(typeof(ObjectPool))]
    public class ObjectPoolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                var objectPool = target as ObjectPool;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Instance Count");
                EditorGUILayout.LabelField(objectPool.InstanceCount.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Available Instance Count");
                EditorGUILayout.LabelField(objectPool.AvailableInstanceCount.ToString());
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
