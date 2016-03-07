using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.Player
{
    /// <summary>
    /// Sent when the number of lives of a player has changed
    /// </summary>
    public struct PlayerLivesChangedMessage
    {
        /// <summary>
        /// The <see cref="Player.PlayerLife"/> component of the player
        /// </summary>
        public PlayerLife PlayerLife;
    }

    /// <summary>
    /// Message sent when a player hits something
    /// </summary>
    public struct PlayerHitMessage
    {
        /// <summary>
        /// The <see cref="Player.PlayerLife"/> component of the player
        /// </summary>
        public PlayerLife PlayerLife;
    }


    /// <summary>
    /// Keeps track of the number of lives of a player
    /// </summary>
    public class PlayerLife : NetworkBehaviour
    {
        /// <summary>
        /// [SyncVar] The number of lives of the player
        /// </summary>
        [SyncVar(hook = "LivesHook")]
        public int Lives = 3;

        /// <summary>
        /// The duration (in seconds) of the [invulnerability] effect after the player hits
        /// something
        /// </summary>
        public float InvulnerabilityDuration = 3;

        /// <summary>
        /// Is the player invulnerable right now?
        /// </summary>
        public bool Invulnerable;


        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {
            // Subscribe to [effect changed] to handle [invulnerability]
            Messenger.Instance.Subscribe<EffectChangedMessage>(this, (sender, message) =>
            {
                if (message.Effect == EffectType.Invulnerability)
                {
                    Invulnerable = message.Duration > 0;
                }
            });
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Adds the given number of lives to the player. The [<see cref="lives"/>] parameter can be
        /// negative to represent damage.
        /// </summary>
        /// <param name="lives">The number of lives to add.</param>
        public void Add(int lives)
        {
            // If [lives] is less than zero than the player hit something
            if (lives < 0)
            {
                // Ignore if [invulnerable]
                if (Invulnerable)
                    return;

                // Send [player hit]
                Messenger.Instance.Send(this, new PlayerHitMessage() { PlayerLife = this });

                // Send [effect changed] to activate [invulnerability]
                Messenger.Instance.Send(this, new EffectChangedMessage() { Effect = EffectType.Invulnerability, Duration = InvulnerabilityDuration });
            }

            // Lives are added on the server only
            CmdAdd(lives);
        }

        /// <summary>
        /// Commands the server to add the given number of lives to the player.
        /// </summary>
        /// <param name="lives">The number of lives to add.</param>
        [Command]
        private void CmdAdd(int lives)
        {
            // [life count] will be synced on all clients and its hook will be invoked
            Lives += lives;
        }

        /// <summary>
        /// The <see cref="Lives"/> [SyncVar] hook.
        /// </summary>
        /// <param name="value">The value of <see cref="Lives"/> on the server.</param>
        private void LivesHook(int value)
        {
            // Update local [lives] value
            Lives = value;

            if (!isLocalPlayer)
                // Send [player hit] for non local players
                Messenger.Instance.Send(this, new PlayerHitMessage() { PlayerLife = this });

            // Send [player lives changed]
            Messenger.Instance.Send(gameObject, new PlayerLivesChangedMessage() { PlayerLife = this });
        }
    }
}