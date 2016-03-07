using PixelView.Time_.Gameplay.Player;
using PixelView.Time_.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay
{
    /// <summary>
    /// The game over message
    /// </summary>
    public struct GameOverMessage
    {
    }


    /// <summary>
    /// The game manager
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
        /// <summary>
        /// Reference to ui objects
        /// </summary>
        [Serializable]
        public struct UIReference
        {
            /// <summary>
            /// The game over
            /// </summary>
            public RectTransform GameOver;
        }


        /// <summary>
        /// The players
        /// </summary>
        public List<GameObject> Players = new List<GameObject>();

        /// <summary>
        /// The reference to ui objects
        /// </summary>
        public UIReference UI;


        private void Awake()
        {
            //
            // Player added
            //
            Messenger.Instance.Subscribe<PlayerAddedMessage>(this, (sender, message) =>
            {
                Players.Add(message.Player);
            });


            //
            // Life count changed
            //
            Messenger.Instance.Subscribe<PlayerLivesChangedMessage>(this, (sender, message) =>
            {
                if (message.PlayerLife.Lives <= 0)
                {
                    // Player died

                    var player = message.PlayerLife.gameObject;

                    Players.Remove(player);

                    if (isServer && Players.Count <= 1)
                    {
                        // One or less players left on server causes game over

                        RpcGameOver();
                    }
                }
            });
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                // Send everyone back to lobby

                FindObjectOfType<LobbyManager>().SendReturnToLobby();
            }
        }

        /// <summary>
        /// Signals game over to all clients
        /// </summary>
        [ClientRpc]
        private void RpcGameOver()
        {
            UI.GameOver.gameObject.SetActive(true);

            Messenger.Instance.Send(gameObject, new GameOverMessage());
        }
    }
}