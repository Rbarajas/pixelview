using PixelView.Time_.Gameplay.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace PixelView.Time_.Networking
{
    public class LobbyManager : NetworkLobbyManager
    {
        [Serializable]
        public class MatchMakerEvent : UnityEvent { }


        [Serializable]
        public struct UIReference
        {
            public RectTransform DisconnectedFromServer;
        }


        public UIReference UI;

        [Space()]
        public MatchMakerEvent OnCreateMatch;

        public MatchMakerEvent OnJoinMatch;

        public MatchMakerEvent OnLeaveLobby;


        private uint m_MatchOwnerId;


        public IEnumerator ListMatches(List<MatchDesc> results)
        {
            StartMatchMaker();

            yield return matchMaker.ListMatches(0, int.MaxValue, string.Empty, OnMatchList);

            results.Clear();
            results.AddRange(matches);
        }

        public void CreateMatch(string matchName, int matchPlayerCount)
        {
            StartMatchMaker();

            matchMaker.CreateMatch(matchName, (uint)matchPlayerCount, false, string.Empty, matchInfo =>
            {
                OnMatchCreate(matchInfo);

                if (matchInfo.success)
                {
                    m_MatchOwnerId = (uint)matchInfo.nodeId;

                    minPlayers = matchPlayerCount;

                    OnCreateMatch.Invoke();
                }
            });
        }

        public void JoinMatch(MatchDesc match)
        {
            StartMatchMaker();

            matchMaker.JoinMatch(match.networkId, string.Empty, matchInfo =>
            {
                OnMatchJoined(matchInfo);

                if (matchInfo.success)
                {
                    m_MatchOwnerId = (uint)match.hostNodeId;

                    minPlayers = match.maxSize;

                    OnJoinMatch.Invoke();
                }
            });
        }

        public void LeaveLobby()
        {
            if (matchInfo == null)
            {
                StopMatchMaker();
                StopHost();

                OnLeaveLobby.Invoke();
            }
            else
            {
                if ((uint)matchInfo.nodeId == m_MatchOwnerId)
                {
                    matchMaker.DestroyMatch(matchInfo.networkId, response =>
                    {
                        StopMatchMaker();
                        StopHost();

                        OnLeaveLobby.Invoke();
                    });
                }
                else
                {
                    StopMatchMaker();
                    StopClient();

                    OnLeaveLobby.Invoke();
                }
            }

            m_MatchOwnerId = 0;
        }

        public void Ready()
        {
            foreach (var lobbyPlayer in lobbySlots)
            {
                if (lobbyPlayer != null && lobbyPlayer.isLocalPlayer)
                {
                    lobbyPlayer.SendReadyToBeginMessage();
                    break;
                }
            }
        }

        public override void OnLobbyClientDisconnect(NetworkConnection conn)
        {
            base.OnLobbyClientDisconnect(conn);

            UI.DisconnectedFromServer.gameObject.SetActive(true);

            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            var lobbyPlayerInfo = lobbyPlayer.GetComponent<PlayerInfo>();
            var gamePlayerInfo = gamePlayer.GetComponent<PlayerInfo>();

            lobbyPlayerInfo.CopyTo(gamePlayerInfo);

            return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        }
    }
}