using PixelView.Time_.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit.Messaging;

namespace PixelView.Time_.UI
{
    public class EffectsScreenManager : MonoBehaviour
    {
        [Serializable]
        public struct UIReference
        {
            public RectTransform[] EffectIcons;

            public Text[] EffectDurations;
        }


        public UIReference UI;


        private void Awake()
        {
            Messenger.Instance.Subscribe<EffectChangedMessage>(this, (sender, message) =>
            {
                UI.EffectIcons[(int)message.Effect].gameObject.SetActive(message.Duration > 0);

                if (message.Duration > 0)
                    UI.EffectDurations[(int)message.Effect].text = message.Duration.ToString("0.0");
            });

            for (var effectIndex = 0; effectIndex < (int)EffectType.Count; ++effectIndex)
                UI.EffectIcons[effectIndex].gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }
    }
}