using UnityEngine;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.Player
{
    /// <summary>
    /// Message sent when a player is added to the game
    /// </summary>
    public struct PlayerAddedMessage
    {
        /// <summary>
        /// The player that was added to the game
        /// </summary>
        public GameObject Player;
    }


    /// <summary>
    /// Message sent when the local player is added to the game
    /// </summary>
    public struct LocalPlayerAddedMessage
    {
        /// <summary>
        /// The local player
        /// </summary>
        public GameObject Player;
    }


    /// <summary>
    /// Player initialization
    /// </summary>
    public class PlayerInit : NetworkBehaviour
    {
        /// <summary>
        /// Called when [start client].
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            // Send [player added]
            //
            // Note that if this player is the local one also the [local player added] message will
            // be sent
            Messenger.Instance.Send(gameObject, new PlayerAddedMessage() { Player = gameObject });
        }

        /// <summary>
        /// Called when [start local player].
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            // Only the local player activates its collider
            GetComponent<Collider>().enabled = true;

            // The camera will follow the local player on the client
            Camera.main.GetComponent<FollowTarget>().Target = transform;

            // Send [local player added]
            //
            // Note that the [player added] message will be sent in any case
            Messenger.Instance.Send(gameObject, new LocalPlayerAddedMessage() { Player = gameObject });
        }
    }
}