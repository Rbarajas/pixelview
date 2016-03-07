using System.Collections.Generic;
using UnityEngine;

namespace UnityToolkit.MemoryManagement
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject Prefab;

        public int InitialCapacity;        


        public int InstanceCount
        {
            get { return m_AllInstances.Count; }
        }

        public int AvailableInstanceCount
        {
            get { return m_AvailableInstances.Count; }
        }


        private ICollection<GameObject> m_AllInstances;

        private Stack<GameObject> m_AvailableInstances;        


        private void Awake()
        {
            m_AllInstances = new HashSet<GameObject>();
            m_AvailableInstances = new Stack<GameObject>();
        }

        private void Start()
        {
            CreateInstances(InitialCapacity);
        }        

        public GameObject GetInstance(bool activateInstance = true)
        {
            if (m_AvailableInstances.Count == 0)
            {
                CreateInstances(m_AllInstances.Count);
            }

            var instance = m_AvailableInstances.Pop();

            if (activateInstance)
                instance.SetActive(true);

            return instance;
        }

        public void RecycleInstance(GameObject instance)
        {
            if (m_AllInstances.Contains(instance))
            {
                m_AvailableInstances.Push(instance);
            }            
        }

        private void CreateInstances(int instanceCount)
        {
            for (var instanceIndex = 0; instanceIndex < instanceCount; ++instanceIndex)
            {
                var instance = Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
                instance.transform.SetParent(transform);
                instance.AddComponent<PooledObject>().Pool = this;
                instance.SetActive(false);

                m_AllInstances.Add(instance);
                m_AvailableInstances.Push(instance);
            }
        }
    }
}
