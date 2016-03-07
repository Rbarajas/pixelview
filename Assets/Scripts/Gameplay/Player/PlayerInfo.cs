using UnityEngine;
using UnityEngine.Networking;

namespace PixelView.Time_.Gameplay.Player
{
    /// <summary>
    /// Player info
    /// 
    /// <para>
    /// This component is attached to both the lobby player and the game player to pass data from
    /// the lobby scene to the play scene
    /// </para>
    /// </summary>
    public class PlayerInfo : NetworkBehaviour
    {
        /// <summary>
        /// [SyncVar] The player index as it appeared in the lobby
        /// </summary>
        [SyncVar]
        public int PlayerIndex = -1;

        /// <summary>
        /// [SyncVar] The player name
        /// </summary>
        [SyncVar]
        public string PlayerName = "Player";

        /// <summary>
        /// [SyncVar] The player color
        /// </summary>
        [SyncVar]
        public Color PlayerColor = Color.white;

        /// <summary>
        /// [SyncVar] The random generator seed used to generate the world
        /// 
        /// All clients must agree on this value to make the generated world the same for all
        /// players
        /// </summary>
        [SyncVar]
        public int WorldSeed;


        public void CopyTo(PlayerInfo other)
        {
            other.PlayerIndex = PlayerIndex;
            other.PlayerName = PlayerName;
            other.PlayerColor = PlayerColor;
            other.WorldSeed = WorldSeed;
        }
    }
}