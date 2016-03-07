using PixelView.Time_.Data;
using PixelView.Time_.Gameplay.Player;
using PixelView.Time_.Gameplay.World;
using PixelView.Time_.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit.Messaging;

namespace PixelView.Time_.UI
{
    /// <summary>
    /// The notification manager
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        /// <summary>
        /// The notifications
        /// </summary>
        [Serializable]
        public struct Notifications_
        {
            /// <summary>
            /// The player hit notification text
            /// </summary>
            public string PlayerHit;

            /// <summary>
            /// The item collected notification text
            /// </summary>
            public string ItemCollected;

            /// <summary>
            /// The theme switched notification text
            /// </summary>
            public string ThemeSwitched;
        }


        /// <summary>
        /// The item database
        /// </summary>
        public ItemDatabase ItemDatabase;

        /// <summary>
        /// The notification prefab
        /// </summary>
        public GameObject NotificationPrefab;

        /// <summary>
        /// The maximum notifications
        /// </summary>
        public int MaxNotifications;

        /// <summary>
        /// The notifications
        /// </summary>
        public Notifications_ Notifications;


        private void Awake()
        {
            // Subscribe to [player hit]
            Messenger.Instance.Subscribe<PlayerHitMessage>(this, (sender, message) =>
            {
                AddNotification(Notifications.PlayerHit, message.PlayerLife.gameObject);
            });

            // Subscribe to [item collected]
            Messenger.Instance.Subscribe<ItemCollectedMessage>(this, (sender, message) =>
            {
                var item = ItemDatabase.FindItemById(message.ItemId);
                var text = string.Format(Notifications.ItemCollected, ColorUtility.ToHtmlStringRGB(item.Color), item.Name);

                AddNotification(text, message.Collector.gameObject);
            });

            // Subscribe to [theme switched]
            Messenger.Instance.Subscribe<ThemeSwitchedMessage>(this, (sender, message) =>
            {
                var text = string.Format(Notifications.ThemeSwitched, message.Theme.Name);

                AddNotification(text, null);
            });
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Adds a notification.
        /// </summary>
        /// <param name="notificationContent">The notification text.</param>
        /// <param name="notifyingPlayer">The player.</param>
        private void AddNotification(string notificationContent, GameObject notifyingPlayer)
        {
            // Instantiate the notification object
            var notification = Instantiate(NotificationPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            // Add to this object and set as first child
            // We assume this object has some kind of auto layout component
            notification.transform.SetParent(transform, false);
            notification.transform.SetAsFirstSibling();

            notification.GetComponent<Text>().text = PrepareNotificationText(notificationContent, notifyingPlayer);

            // Destroy oldest notification if notification count is greater than max notifications
            if (transform.childCount > MaxNotifications)
            {
                Destroy(transform.GetChild(MaxNotifications).gameObject);
            }
        }

        private string PrepareNotificationText(string notificationContent, GameObject notifyingPlayer)
        {
            var text = string.Empty;

            if (notifyingPlayer != null)
            {
                var playerInfo = notifyingPlayer.GetComponent<PlayerInfo>();

                text += string.Format("[<color=#{0}>{1}</color>] ",
                                      ColorUtility.ToHtmlStringRGB(playerInfo.PlayerColor),
                                      playerInfo.PlayerName);
            }

            text += notificationContent;

            return text;
        }

        
    }
}