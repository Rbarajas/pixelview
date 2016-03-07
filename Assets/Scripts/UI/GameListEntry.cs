using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace PixelView.Time_.UI
{
    public class GameListEntry : MonoBehaviour
    {
        [System.Serializable]
        public class JoinGameEvent : UnityEvent<MatchDesc> { }


        /// <summary>
        /// References the required UI objects
        /// </summary>
        [System.Serializable]
        public struct UIReference
        {
            public Text GameNameText;

            public Text PlayerCountText;

            public Button JoinButton;
        }


        public UIReference UI;

        public JoinGameEvent OnJoinGame;


        public void Populate(MatchDesc match)
        {
            UI.GameNameText.text = match.name;
            UI.PlayerCountText.text = string.Format("{0}/{1}", match.currentSize, match.maxSize);

            UI.JoinButton.onClick.RemoveAllListeners();
            UI.JoinButton.onClick.AddListener(() => OnJoinGame.Invoke(match));
        }
    }
}