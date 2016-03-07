using System;
using System.Collections.Generic;

namespace UnityToolkit.Messaging
{
    public class Messenger : Singleton<Messenger>
    {
        private IDictionary<Type, IDictionary<object, object>> m_SubscriptionsByMessageType;

        private object[] m_AuxBuf;


        private void Awake()
        {
            m_SubscriptionsByMessageType = new Dictionary<Type, IDictionary<object, object>>();

            m_AuxBuf = new object[256];
        }

        public bool Send<T>(object sender, T message)
        {
            IDictionary<object, object> callbacksBySubscriber;

            if (m_SubscriptionsByMessageType.TryGetValue(typeof(T), out callbacksBySubscriber))
            {
                if (m_AuxBuf.Length < callbacksBySubscriber.Count)
                    m_AuxBuf = new object[callbacksBySubscriber.Count * 2];
                callbacksBySubscriber.Values.CopyTo(m_AuxBuf, 0);

                var callbackCount = callbacksBySubscriber.Count;
                for (var callbackIndex = 0; callbackIndex < callbackCount; ++callbackIndex)
                {
                    var callback = (Action<object, T>)m_AuxBuf[callbackIndex];

                    callback(sender, message);
                }

                Array.Clear(m_AuxBuf, 0, callbackCount);

                return callbackCount > 0;
            }

            return false;
        }

        public void Subscribe<T>(object subscriber, Action<object, T> callback)
        {
            IDictionary<object, object> callbacksBySubscriber;

            if (!m_SubscriptionsByMessageType.TryGetValue(typeof(T), out callbacksBySubscriber))
            {
                callbacksBySubscriber = new Dictionary<object, object>();

                m_SubscriptionsByMessageType.Add(typeof(T), callbacksBySubscriber);
            }

            callbacksBySubscriber.Add(subscriber, callback);
        }

        public void Unsubscribe(object subscriber)
        {
            foreach (var callbacksBySubscriber in m_SubscriptionsByMessageType.Values)
            {
                callbacksBySubscriber.Remove(subscriber);
            }
        }

        public bool Unsubscribe<T>(object subscriber)
        {
            IDictionary<object, object> callbacksBySubscriber;

            if (m_SubscriptionsByMessageType.TryGetValue(typeof(T), out callbacksBySubscriber))
            {
                return callbacksBySubscriber.Remove(subscriber);
            }

            return false;
        }

        public void UnsubscribeAll()
        {
            m_SubscriptionsByMessageType.Clear();
        }

        public void UnsubscribeAll<T>()
        {
            IDictionary<object, object> callbacksBySubscriber;

            if (m_SubscriptionsByMessageType.TryGetValue(typeof(T), out callbacksBySubscriber))
            {
                callbacksBySubscriber.Clear();
            }
        }
    }
}