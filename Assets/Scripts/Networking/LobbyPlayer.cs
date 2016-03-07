using PixelView.Time_.Gameplay.Player;
using PixelView.Time_.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Networking
{
    /// <summary>
    /// Message sent when a lobby player is either added or removed
    /// </summary>
    public struct LobbyPlayerAddedOrRemoved
    {
        /// <summary>
        /// The lobby player component
        /// </summary>
        public LobbyPlayer Player;

        /// <summary>
        /// If true the player was added, otherwise the player was removed.
        /// </summary>
        public bool PlayerAdded;
    }


    /// <summary>
    /// Message sent the ready state of a lobby player changed
    /// </summary>
    public struct LobbyPlayerReadyChanged
    {
        /// <summary>
        /// The lobby player component
        /// </summary>
        public LobbyPlayer Player;

        /// <summary>
        /// The ready state of the lobby player
        /// </summary>
        public bool Ready;
    }


    /// <summary>
    /// Represents a player in the lobby
    /// </summary>
    [RequireComponent(typeof(PlayerInfo))]
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        /// <summary>
        /// The name of players whose infos were not received yet.
        /// </summary>
        public string UnknownPlayerName = "???";

        /// <summary>
        /// The color of players whose infos were not received yet.
        /// </summary>
        public Color UnknownPlayerColor = Color.white;


        // Reference to the required PlayerInfo component
        private PlayerInfo m_PlayerInfo;


        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {            
            // Subscribe to [chat message sent] to send the new message to the server (invoking
            // commands on the server requires player's authority)
            Messenger.Instance.Subscribe<ChatMessageSent>(this, (sender, message) =>
            {
                // Handle the message only on the local lobby player instance
                if (!isLocalPlayer)
                    return;

                // Add the player's name as prefix (using the relevant color)
                var text = string.Format("[<color=#{0}>{1}</color>] {2}",
                                         ColorUtility.ToHtmlStringRGB(m_PlayerInfo.PlayerColor),
                                         m_PlayerInfo.PlayerName,
                                         message.Text);

                // Send the message to the server so that all players can receive an RPC back
                CmdSendChatMessage(text);
            });

            // Get the required PlayerInfo component
            m_PlayerInfo = GetComponent<PlayerInfo>();
        }

        /// <summary>
        /// Called when [start client].
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            // Store the slot index for use in game
            m_PlayerInfo.PlayerIndex = slot;

            // Send [lobby player added or removed] to inform the the player was added
            Messenger.Instance.Send(this, new LobbyPlayerAddedOrRemoved() { Player = this, PlayerAdded = true });
        }

        /// <summary>
        /// Called when [start local player].
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            // First player in lobby determines the world seed
            // For player hosted games (no dedicated server) this is the host player
            if (slot == 0)
            {
                // This is a SyncVar so all clients will receive it
                m_PlayerInfo.WorldSeed = Random.seed;
            }

            // Send local player infos (name and color) to the server
            CmdSetPlayerCustomizationInfo(GameSettings.Instance.PlayerName, (byte)GameSettings.Instance.PlayerColorIndex);
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        private void OnDestroy()
        {
            // Send [lobby player added or removed] to inform the the player was removed
            Messenger.Instance.Send(this, new LobbyPlayerAddedOrRemoved() { Player = this, PlayerAdded = false });

            // Unsubscribe from all message notifications
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Called when [client enter lobby].
        /// </summary>
        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            // Send [player ready changed] to update the UI accordingly
            //
            // The same message will be sent if the ready state changes (see [on client ready])
            Messenger.Instance.Send(this, new LobbyPlayerReadyChanged() { Player = this, Ready = readyToBegin });
        }

        /// <summary>
        /// Called when [client ready].
        /// </summary>
        /// <param name="readyState">The ready state of the player.</param>
        public override void OnClientReady(bool readyState)
        {
            base.OnClientReady(readyState);

            // Send [player ready changed] to update the UI accordingly
            Messenger.Instance.Send(this, new LobbyPlayerReadyChanged() { Player = this, Ready = readyToBegin });
        }

        /// <summary>
        /// Sends player name and color to the server.
        /// </summary>
        /// <param name="playerName">Player name.</param>
        /// <param name="playerColorIndex">Player color index.</param>
        [Command]
        private void CmdSetPlayerCustomizationInfo(string playerName, byte playerColorIndex)
        {
            // Set on the server, these are SyncVars and will be updated on all the clients
            m_PlayerInfo.PlayerName = playerName;
            m_PlayerInfo.PlayerColor = GameSettings.Instance.SelectableColors[playerColorIndex];
        }

        /// <summary>
        /// Sends a chat message to the server.
        /// </summary>
        /// <param name="text">The message text.</param>
        [Command]
        private void CmdSendChatMessage(string text)
        {
            // Broadcast to all clients
            RpcReceiveChatMessage(text);
        }

        /// <summary>
        /// Receives chat messages from the server.
        /// </summary>
        /// <param name="text">The  message text.</param>
        [ClientRpc]
        private void RpcReceiveChatMessage(string text)
        {
            // Send [chat message received] to update the UI
            Messenger.Instance.Send(this, new ChatMessageReceived() { Text = text });
        }
    }
}