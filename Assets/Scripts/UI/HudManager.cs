using PixelView.Time_.Data;
using PixelView.Time_.Gameplay.Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit.Messaging;

namespace PixelView.Time_.UI
{
    /// <summary>
    /// The hud manager
    /// </summary>
    public class HudManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to ui objects
        /// </summary>
        [Serializable]
        public struct UIReference
        {
            public Image Overlay;
        }


        /// <summary>
        /// The item database
        /// </summary>
        public ItemDatabase ItemDatabase;

        /// <summary>
        /// The player hit overlay color
        /// </summary>
        public Color PlayerHitOverlayColor = new Color(1, 1, 1, 0.2f);

        /// <summary>
        /// The reference to ui objects
        /// </summary>
        public UIReference UI;


        private void Awake()
        {
            //
            // Life count changed
            //
            Messenger.Instance.Subscribe<PlayerHitMessage>(this, (sender, message) =>
            {
                if (message.PlayerLife.isLocalPlayer)
                    // Local player hit
                    StartCoroutine(ShowOverlay(PlayerHitOverlayColor, 1));
            });


            //
            // Item collected
            //
            Messenger.Instance.Subscribe<ItemCollectedMessage>(this, (sender, message) =>
            {
                if (message.Collector.isLocalPlayer)
                {
                    // Local player collected the item

                    var item = ItemDatabase.FindItemById(message.ItemId);

                    StartCoroutine(ShowOverlay(item.Color, 0.5f));
                }
            });
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Shows the overlay.
        /// </summary>
        /// <param name="color">The overlay color.</param>
        /// <param name="duration">The overlay duration.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator ShowOverlay(Color color, float duration)
        {
            UI.Overlay.gameObject.SetActive(true);

            var a = color.a;
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                color.a = Mathf.Lerp(a, 0, t);

                UI.Overlay.color = color;

                yield return null;
            }

            UI.Overlay.gameObject.SetActive(false);
        }
    }
}