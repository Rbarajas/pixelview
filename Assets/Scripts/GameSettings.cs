using UnityEngine;
using UnityToolkit;

namespace PixelView.Time_
{
    /// <summary>
    /// Stores Game Settings using Unity's PlayerPrefs
    /// </summary>
    public class GameSettings : Singleton<GameSettings>
    {
        // Key for the [volume] setting
        private static readonly string VolumeKey = "GameSettings.VolumeKey";

        // Key for the [sensitivity] setting
        private static readonly string SensitivityKey = "GameSettings.SensitivityKey";

        // Key for the [player name] setting
        private static readonly string PlayerNameKey = "GameSettings.PlayerNameKey";

        // Key for the [player color index] setting
        private static readonly string PlayerColorIndexKey = "GameSettings.PlayerColorIndexKey";


        /// <summary>
        /// The colors a player can select
        /// </summary>
        public Color[] SelectableColors = new Color[] { Color.white };


        /// <summary>
        /// Gets or sets the [volume] setting
        /// 
        /// The value is persisted between game executions
        /// </summary>
        public float Volume
        {
            get { return PlayerPrefs.GetFloat(VolumeKey, 0.5f); }
            set { PlayerPrefs.SetFloat(VolumeKey, value); }
        }

        /// <summary>
        /// Gets or sets the [sensitivity] setting
        /// 
        /// The value is persisted between game executions
        /// </summary>
        public float Sensitivity
        {
            get { return PlayerPrefs.GetFloat(SensitivityKey, 0.5f); }
            set { PlayerPrefs.SetFloat(SensitivityKey, value); }
        }

        /// <summary>
        /// Gets or sets the [player name] setting
        /// 
        /// The value is persisted between game executions
        /// </summary>
        public string PlayerName
        {
            get { return PlayerPrefs.GetString(PlayerNameKey, "Player"); }
            set { PlayerPrefs.SetString(PlayerNameKey, value); }
        }

        /// <summary>
        /// Gets or sets the [player color] setting
        /// 
        /// The value is persisted between game executions
        /// </summary>
        public int PlayerColorIndex
        {
            get { return PlayerPrefs.GetInt(PlayerColorIndexKey, 0); }
            set { PlayerPrefs.SetInt(PlayerColorIndexKey, value); }
        }

        /// <summary>
        /// Gets the selected player color
        /// </summary>
        public Color PlayerColor
        {
            get { return SelectableColors[PlayerColorIndex]; }
        }
    }
}