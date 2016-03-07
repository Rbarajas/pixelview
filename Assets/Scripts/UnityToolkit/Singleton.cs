using UnityEngine;

namespace UnityToolkit
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {        
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    if ((m_Instance = FindObjectOfType<T>()) == null)
                        m_Instance = new GameObject(typeof(T).Name + " (Singleton)").AddComponent<T>();

                    DontDestroyOnLoad(m_Instance.gameObject);
                }
                else
                {
                    var sceneInstances = FindObjectsOfType<T>();
                    for (var i = 1; i < sceneInstances.Length; ++i)
                        Destroy(sceneInstances[i]);
                }

                return m_Instance;
            }
        }


        private static T m_Instance;


        private void OnDestroy()
        {
            m_Instance = null;
        }
    }
}
