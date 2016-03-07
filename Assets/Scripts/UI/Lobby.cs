using PixelView.Time_.Networking;
using System.Collections;
using UnityEngine;

namespace PixelView.Time_.UI
{
    public class Lobby : MonoBehaviour
    {
        [System.Serializable]
        public struct UIReference
        {
            public RectTransform InfoOverlay;
        }


        public LobbyManager LobbyManager;

        public UIReference UI;

        private void OnEnable()
        {
            UI.InfoOverlay.gameObject.SetActive(false);
        }

        public void Training()
        {
            LobbyManager.minPlayers = 1;
            LobbyManager.StartHost();
        }
    }
}