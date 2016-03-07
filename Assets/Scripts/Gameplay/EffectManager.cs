using UnityEngine;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay
{
    public enum EffectType
    {
        Blind,

        Ftl,

        InvertedInput,

        Invulnerability,

        Panic,

        Count
    }


    public struct EffectChangedMessage
    {
        public EffectType Effect;

        public float Duration;
    }


    public class EffectManager : NetworkBehaviour
    {
        private float[] m_Durations = new float[(int)EffectType.Count];


        private void Awake()
        {
            Messenger.Instance.Subscribe<EffectChangedMessage>(this, (sender, message) =>
            {
                if (sender == this)
                    return;

                if (message.Duration > m_Durations[(int)message.Effect])
                {
                    m_Durations[(int)message.Effect] = message.Duration;
                }
            });
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        private void Update()
        {
            for (var i = 0; i < (int)EffectType.Count; ++i)
            {
                if (m_Durations[i] <= 0)
                    continue;

                m_Durations[i] = Mathf.Max(m_Durations[i] - Time.deltaTime, 0);

                Messenger.Instance.Send(this, new EffectChangedMessage() { Effect = (EffectType)i, Duration = m_Durations[i] });
            }
        }
    }
}