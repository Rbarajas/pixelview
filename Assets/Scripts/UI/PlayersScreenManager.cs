using PixelView.Time_.Gameplay.Player;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit.Messaging;

namespace PixelView.Time_.UI
{
    public class PlayersScreenManager : MonoBehaviour
    {
        [Serializable]
        public struct UIReference
        {
            public Text[] PlayerNames;

            public RectTransform[] PlayerLives;
        }


        public UIReference UI;


        private void Awake()
        {
            Messenger.Instance.Subscribe<PlayerAddedMessage>(this, (sender, message) =>
            {
                var playerInfo = message.Player.GetComponent<PlayerInfo>();
                var playerIndex = playerInfo.PlayerIndex;

                UI.PlayerNames[playerIndex].gameObject.SetActive(true);
                UI.PlayerNames[playerIndex].text = playerInfo.PlayerName;
                UI.PlayerNames[playerIndex].color = playerInfo.PlayerColor;

                UI.PlayerLives[playerIndex].gameObject.SetActive(true);
                UpdatePlayerLives(message.Player);
            });

            Messenger.Instance.Subscribe<PlayerLivesChangedMessage>(this, (sender, message) =>
            {
                UpdatePlayerLives(message.PlayerLife.gameObject);
            });

            var maxPlayers = 4;
            for (var playerIndex = 0; playerIndex < maxPlayers; ++playerIndex)
            {
                UI.PlayerNames[playerIndex].gameObject.SetActive(false);
                UI.PlayerLives[playerIndex].gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        private void UpdatePlayerLives(GameObject player)
        {
            var playerIndex = player.GetComponent<PlayerInfo>().PlayerIndex;
            var playerLife = player.GetComponent<PlayerLife>();

            var playerLives = UI.PlayerLives[playerIndex];

            for (var i = 0; i < playerLives.childCount; ++i)
            {
                playerLives.GetChild(i).gameObject.SetActive(i < playerLife.Lives);
            }
        }
    }
}