using PixelView.Time_.Gameplay.Player;
using UnityEngine;

namespace PixelView.Time_.Gameplay
{
    /// <summary>
    /// To attach to those objects that apply damage to the players when hit
    /// </summary>
    public class ApplyDamage : MonoBehaviour
    {
        /// <summary>
        /// How much damage to apply (number of lives lost)
        /// </summary>
        public int Damage = 1;


        /// <summary>
        /// Called when [trigger enter].
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            var playerLife = other.GetComponent<PlayerLife>();

            if (playerLife != null)
            {
                playerLife.Add(-1 * Damage);
            }
        }
    }
}