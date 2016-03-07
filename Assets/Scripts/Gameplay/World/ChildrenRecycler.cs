using UnityEngine;

namespace PixelView.Time_.Gameplay.World
{
    /// <summary>
    /// Recycles children of the object when they are way back the main camera
    /// </summary>
    public class ChildrenRecycler : MonoBehaviour
    {
        /// <summary>
        /// The time interval between recycles
        /// </summary>
        public float TimeInterval = 1;

        /// <summary>
        /// The distance a child has to get behind the main camera to be recycled
        /// </summary>
        public float RecycleDistance = 100;


        /// <summary>
        /// Called when [start].
        /// </summary>
        private void Start()
        {
            InvokeRepeating("Recycle", TimeInterval, TimeInterval);
        }

        /// <summary>
        /// Does the recycling.
        /// </summary>
        private void Recycle()
        {
            foreach (Transform child in transform)
            {
                // Check child position and recycle it if appropriate
                if (Camera.main.transform.position.z - child.position.z >= RecycleDistance)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}