using PixelView.Time_.Gameplay.Player;
using UnityEngine;

namespace PixelView.Time_.Gameplay
{
    public class Item : MonoBehaviour
    {
        [HideInInspector]
        public int ItemId;


        private void OnTriggerEnter(Collider other)
        {
            var collector = other.GetComponent<PlayerCollector>();

            if (collector != null)
            {
                collector.Collect(ItemId);

                Destroy(gameObject);
            }
        }
    }
}