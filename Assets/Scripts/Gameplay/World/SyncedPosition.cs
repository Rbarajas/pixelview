using UnityEngine;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.World
{
    public struct DistanceTravelledChanged
    {
        public float DistanceTravelled;
    }


    public class SyncedPosition : NetworkBehaviour
    {
        /// <summary>
        /// [SyncVar] The distance travelled
        /// </summary>
        [SyncVar(hook = "DistanceTravelledHook")]
        public float DistanceTravelled;

        /// <summary>
        /// The speed of <see cref="DistanceTravelled"/> variation
        /// </summary>
        public float Speed = 35;

        /// <summary>
        /// The maximum value of <see cref="Speed"/>
        /// </summary>
        public float MaxSpeed = 45;

        /// <summary>
        /// The increment of <see cref="Speed"/> when the [ftl] effect is active
        /// </summary>
        public float FtlSpeedBoost = 20;

        /// <summary>
        /// The acceleration applied to <see cref="Speed"/> every frame
        /// </summary>
        public float Acceleration = 10f / 120;


        // Should the ftl speed boost be applied?
        private bool m_Ftl;


        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {
            // Subscribe to [effect changed] to handle [ftl]
            Messenger.Instance.Subscribe<EffectChangedMessage>(this, (sender, message) =>
            {
                if (message.Effect == EffectType.Ftl)
                {
                    m_Ftl = message.Duration > 0;
                }
            });
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        private void Update()
        {
            // Accelerate
            Speed += Acceleration * Time.deltaTime;

            // Cap the speed to allowed maxmimum
            if (Speed > MaxSpeed) Speed = MaxSpeed;

            // Update the distance travelled
            DistanceTravelled += Speed * Time.deltaTime;
            // Increase the distance even more if ftl is active
            if (m_Ftl) DistanceTravelled += FtlSpeedBoost * Time.deltaTime;

            // Send [distance travelled changed]
            Messenger.Instance.Send(this, new DistanceTravelledChanged() { DistanceTravelled = this.DistanceTravelled });
        }

        /// <summary>
        /// The <see cref="DistanceTravelled"/> [SyncVar] hook.
        /// </summary>
        /// <param name="value">The value of <see cref="DistanceTravelled"/> on the server.</param>
        private void DistanceTravelledHook(float value)
        {
            // Lerping should slightly reduce the jitter
            DistanceTravelled = Mathf.Lerp(DistanceTravelled, value, 0.1f);
        }
    }
}