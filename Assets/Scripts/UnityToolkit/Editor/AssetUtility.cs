using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityToolkit
{
    public static class AssetUtility
    {
        public static T CreateAsset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = Path.GetDirectoryName(path);
            }
            path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(T).Name + ".asset");

            AssetDatabase.CreateAsset(asset, path);

            return asset;
        }
    }
}