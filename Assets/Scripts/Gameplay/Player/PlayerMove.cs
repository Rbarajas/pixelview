using PixelView.Time_.Gameplay.World;
using UnityEngine;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.Player
{
    /// <summary>
    /// Player movement
    /// </summary>
    public class PlayerMove : NetworkBehaviour
    {
        /// <summary>
        /// The speed of the player
        /// </summary>
        public float Speed = 1;

        /// <summary>
        /// The responsivness indicates how quickly the movement direction reacts to input
        /// </summary>
        [Range(0, 1)]
        public float Responsivness = 0.1f;

        /// <summary>
        /// The allowed movement radius
        /// </summary>
        public float AllowedMovementRadius = 100;


        // Mouse movement deltas computed last frame
        private Vector3 m_LastMouseDeltas;

        // Is the input response inverted? 1 for no and -1 for yes
        private float m_InvertedInput = 1;


        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {
            // Subscribe to [synced position updated] to update the position of the player along the
            // forward direction (world Z axis)
            Messenger.Instance.Subscribe<DistanceTravelledChanged>(this, (sender, message) =>
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    message.DistanceTravelled);
            });

            // Subscribe to [effect changed] to handle [inverted input]
            Messenger.Instance.Subscribe<EffectChangedMessage>(this, (sender, message) =>
            {
                if (message.Effect == EffectType.InvertedInput)
                {
                    m_InvertedInput = message.Duration > 0 ? -1 : 1;
                }
            });
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        private void OnDestroy()
        {
            // Remove any callback associated with this object to prevent errors
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        private void Update()
        {
            // Only the local player can be controller
            if (isLocalPlayer)
            {
                // Read mouse movement deltas frome the [input manager]
                var mouseDeltaX = Input.GetAxis("Mouse X");
                var mouseDeltaY = Input.GetAxis("Mouse Y");

                // Compute new movement direction taking [responsivness] into account
                m_LastMouseDeltas.x = Mathf.Lerp(m_LastMouseDeltas.x, mouseDeltaX, Responsivness);
                m_LastMouseDeltas.y = Mathf.Lerp(m_LastMouseDeltas.y, mouseDeltaY, Responsivness);

                // Compute new position
                var updatedPosition = transform.position + m_LastMouseDeltas * m_InvertedInput * Speed;

                if (((Vector2)updatedPosition).sqrMagnitude > AllowedMovementRadius * AllowedMovementRadius)
                {
                    updatedPosition = ((Vector2)updatedPosition).normalized * AllowedMovementRadius;
                    updatedPosition.z = transform.position.z;
                }

                transform.position = updatedPosition;
            }
        }
    }
}