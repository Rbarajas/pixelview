using System.Collections.Generic;
using UnityEngine;

namespace UnityToolkit.MemoryManagement
{
    public class ObjectPooler : Singleton<ObjectPooler>
    {
        public IEnumerable<ObjectPool> Pools
        {
            get { return m_Pools; }
        }


        private ObjectPool[] m_Pools;

        private IDictionary<GameObject, ObjectPool> m_PoolsByPrefab;


        private void Awake()
        {
            m_Pools = GetComponentsInChildren<ObjectPool>();

            m_PoolsByPrefab = new Dictionary<GameObject, ObjectPool>(m_Pools.Length);
            foreach (var pool in m_Pools)
            {
                m_PoolsByPrefab.Add(pool.Prefab, pool);
            }
        }

        public GameObject GetInstance(GameObject prefab, bool activateInstance = true)
        {
            ObjectPool pool;

            if (m_PoolsByPrefab.TryGetValue(prefab, out pool))
            {
                return pool.GetInstance(activateInstance);
            }

            return null;
        }

        public void RecycleInstance(GameObject instance)
        {
            var pool = instance.GetComponent<PooledObject>().Pool;

            pool.RecycleInstance(instance);
        }
    }
}
