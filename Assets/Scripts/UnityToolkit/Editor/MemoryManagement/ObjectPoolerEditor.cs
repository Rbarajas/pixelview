using UnityEditor;

namespace UnityToolkit.MemoryManagement
{
    [CustomEditor(typeof(ObjectPooler))]
    public class ObjectPoolerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                var objectPooler = target as ObjectPooler;

                foreach (var pool in objectPooler.Pools)
                {                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(pool.Prefab.name);
                    EditorGUILayout.LabelField(pool.AvailableInstanceCount + "/" + pool.InstanceCount);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}
