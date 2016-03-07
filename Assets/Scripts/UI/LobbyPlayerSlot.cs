using PixelView.Time_.Gameplay.Player;
using PixelView.Time_.Networking;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit.Messaging;

namespace PixelView.Time_.UI
{
    /// <summary>
    /// Controls UI logic of lobby player slots
    /// </summary>
    public class LobbyPlayerSlot : MonoBehaviour
    {
        /// <summary>
        /// Reference to required UI objects
        /// </summary>
        [Serializable]
        public struct UIRerefence
        {
            /// <summary>
            /// The UI text that displays the player's name
            /// </summary>
            public Text PlayerNameText;

            /// <summary>
            /// The UI text that displays the player's ready state
            /// </summary>
            public Text ReadyStateText;
        }


        /// <summary>
        /// The text to display when the player is ready
        /// </summary>
        public string ReadyText = "READY";

        /// <summary>
        /// The color of the ready state text when the player is ready
        /// </summary>
        public Color ReadyColor = Color.green;

        /// <summary>
        /// The text to display when the player is not ready
        /// </summary>
        public string NotReadyText = "NOT READY";

        /// <summary>
        /// The color of the ready state text when the player is not ready
        /// </summary>
        public Color NotReadyColor = Color.red;

        /// <summary>
        /// Reference to required UI objects
        /// </summary>
        public UIRerefence UI;


        // The lobby player GameObject
        private GameObject m_Player;


        /// <summary>
        /// Called when [awake]
        /// </summary>
        private void Awake()
        {
            // Subscribe to [lobby plated added or removed] to obtain a reference to the lobby
            // player belonging to this slot
            Messenger.Instance.Subscribe<LobbyPlayerAddedOrRemoved>(this, (sender, message) =>
            {
                // Not the player belonging to this slot?
                if (transform.GetSiblingIndex() != message.Player.slot)
                    return;

                // Store a reference to the GameObject
                m_Player = message.Player.gameObject;

                // Activate the UI since we now have a lobby player to display
                gameObject.SetActive(message.PlayerAdded);
            });

            // Start inactive as we don't have a reference to the lobby player yet
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when [update]
        /// </summary>
        private void Update()
        {
            // Refreshes the UI
            //
            // NOTE
            // Calling this method every frame is inefficient but since we are talking about
            // updating the non in-game UI there isn't really any need to fix it
            Refresh();
        }

        /// <summary>
        /// Refreshes the UI
        /// </summary>
        public void Refresh()
        {
            // Update player name text and color
            var playerInfo = m_Player.GetComponent<PlayerInfo>();
            UI.PlayerNameText.text = playerInfo.PlayerName;
            UI.PlayerNameText.color = playerInfo.PlayerColor;

            // Upldate player ready state text and color
            var lobbyPlayer = m_Player.GetComponent<LobbyPlayer>();
            UI.ReadyStateText.text = lobbyPlayer.readyToBegin ? ReadyText : NotReadyText;
            UI.ReadyStateText.color = lobbyPlayer.readyToBegin ? ReadyColor : NotReadyColor;
        }
    }
}