using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit.Messaging;

namespace PixelView.Time_.UI
{
    /// <summary>
    /// Message sent when a new chat message should be sent to the server
    /// </summary>
    public struct ChatMessageSent
    {
        /// <summary>
        /// The message text
        /// </summary>
        public string Text;
    }

    /// <summary>
    /// Message sent when a chat message is received from the server
    /// </summary>
    public struct ChatMessageReceived
    {
        /// <summary>
        /// The message text
        /// </summary>
        public string Text;
    }


    /// <summary>
    /// Controls UI logic of the lobby Chat
    /// </summary>
    public class Chat : MonoBehaviour
    {
        /// <summary>
        /// Reference to the required UI objects
        /// </summary>
        [System.Serializable]
        public struct UIReference
        {
            /// <summary>
            /// The input field that allows players to type messages
            /// </summary>
            public InputField ChatInput;

            /// <summary>
            /// The UI object containing the chat messages
            /// </summary>
            public RectTransform ChatMessageList;
        }


        /// <summary>
        /// The maximum number of messages displayed into the chat window
        /// </summary>
        public int MaxMessages = 10;

        /// <summary>
        /// The prefab representing chat messages
        /// </summary>
        public GameObject ChatMessageTextPrefab;

        /// <summary>
        /// Reference to the required UI objects
        /// </summary>
        public UIReference UI;


        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {         
            /// Subscribe to [chat message received] to update the UI               
            Messenger.Instance.Subscribe<ChatMessageReceived>(this, (sender, message) =>
            {
                // Add the message to the UI
                RefreshMessageList(message.Text);
            });

            // Listen to [on end edit] to send the message to the server
            UI.ChatInput.onEndEdit.AddListener(AddMessage);
        }

        /// <summary>
        /// Called when [enable].
        /// </summary>
        private void OnEnable()
        {
            // Remove old messages if any when the lobby screen is open
            foreach (Transform child in UI.ChatMessageList)
                Destroy(child.gameObject);
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        private void OnDestroy()
        {
            // Unsubscribe from all message notifications
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Adds a message.
        /// </summary>
        /// <param name="text">The new message text.</param>
        private void AddMessage(string text)
        {
            // Clear the input field text
            UI.ChatInput.text = string.Empty;
            // Give again focus to the textbox
            UI.ChatInput.ActivateInputField();


            // Send [chat message sent] to send the message to the server (see [lobby player])
            Messenger.Instance.Send(this, new ChatMessageSent() { Text = text });
        }

        /// <summary>
        /// Refreshes the message list UI.
        /// </summary>
        /// <param name="text">The new message text.</param>
        private void RefreshMessageList(string text)
        {
            // Instantiate the chat message object
            var chatMessage = Instantiate(ChatMessageTextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            // Set its text
            chatMessage.GetComponent<Text>().text = text;

            // Add it to the message list
            chatMessage.transform.SetParent(UI.ChatMessageList, false);

            // If current message count exceeds the maximum value
            if (UI.ChatMessageList.childCount > MaxMessages)
            {
                // Remove oldest message
                Destroy(UI.ChatMessageList.GetChild(0).gameObject);
            }
        }
    }
}