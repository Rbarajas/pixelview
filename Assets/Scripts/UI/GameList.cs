using PixelView.Time_.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace PixelView.Time_.UI
{
    public class GameList : MonoBehaviour
    {
        /// <summary>
        /// Reference to the required UI objects
        /// </summary>
        [System.Serializable]
        public struct UIReference
        {
            /// <summary>
            /// Textbox used to input the name of game to create
            /// </summary>
            public InputField GameNameTextbox;

            /// <summary>
            /// The prefab of a game list entry object
            /// </summary>
            public GameObject GameListEntryPrefab;

            /// <summary>
            /// Content pane to put game entries into
            /// </summary>
            public RectTransform GameListContent;
        }


        /// <summary>
        /// The Lobby Manager required to perform network operations
        /// </summary>
        public LobbyManager LobbyManager;

        /// <summary>
        /// Minimum characters length of a game's name
        /// </summary>
        public int MinGameNameLength = 6;

        /// <summary>
        /// Reference to the required UI objects
        /// </summary>
        public UIReference UI;


        // List of games we know about
        private List<MatchDesc> m_Matches = new List<MatchDesc>();


        /// <summary>
        /// Called when [enable]
        /// </summary>
        private void OnEnable()
        {
            m_Matches.Clear();
            Populate();

            StartCoroutine(RefreshList());
        }

        /// <summary>
        /// Refreshes the list of available games
        /// </summary>
        public IEnumerator RefreshList()
        {
            while (true)
            {
                yield return LobbyManager.ListMatches(m_Matches);

                Populate();

                yield return new WaitForSeconds(5);
            }
        }

        /// <summary>
        /// Populates the list UI with the available games
        /// </summary>
        public void Populate()
        {
            // Destroy previous entries
            foreach (Transform entry in UI.GameListContent)
                Destroy(entry.gameObject);

            // Add one entry per game
            foreach (var match in m_Matches)
            {
                // Skip full games
                if (match.currentSize >= match.maxSize)
                    continue;

                // Create entry and put it into the list UI
                var entry = Instantiate(UI.GameListEntryPrefab);
                entry.transform.SetParent(UI.GameListContent, false);

                // Populate the entry with match data
                entry.GetComponent<GameListEntry>().Populate(match);
                entry.GetComponent<GameListEntry>().OnJoinGame.AddListener(matchToJoin => LobbyManager.JoinMatch(matchToJoin));
            }
        }

        /// <summary>
        /// Creates a new game
        /// </summary>
        public void CreateGame()
        {
            var gameName = UI.GameNameTextbox.text;

            if (gameName.Length >= MinGameNameLength)
            {
                LobbyManager.CreateMatch(gameName, 2);
                UI.GameNameTextbox.text = string.Empty;
            }
        }
    }
}