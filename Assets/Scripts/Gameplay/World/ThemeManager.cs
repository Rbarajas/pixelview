using PixelView.Time_.Data;
using PixelView.Time_.Gameplay.Player;
using PixelView.Time_.Utility;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.World
{
    /// <summary>
    /// Message sent when the current theme changes.
    /// </summary>
    public struct ThemeSwitchedMessage
    {
        /// <summary>
        /// The new theme
        /// </summary>
        public ThemeData Theme;
    }


    /// <summary>
    /// Manages switching between themes.
    /// </summary>
    public class ThemeManager : NetworkBehaviour
    {
        /// <summary>
        /// Reference to UI objects.
        /// </summary>
        [Serializable]
        public struct UIReference
        {
            /// <summary>
            /// The warp charge slider.
            /// </summary>
            public Slider WarpCharge;
        }


        /// <summary>
        /// The chunk database
        /// </summary>
        public ChunkDatabase ChunkDatabase;

        /// <summary>
        /// Reference to UI objects.
        /// </summary>
        public UIReference UI;

        /// <summary>
        /// The current theme.
        /// </summary>
        [HideInInspector]
        public ThemeData CurrentTheme;

        /// <summary>
        /// The number of warp charges required to switch theme
        /// </summary>
        public int ChargesToSwitchTheme = 10;


        [SyncVar(hook = "WarpChargeHook")]
        private byte m_WarpCharge;


        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {
            // Subscribe to [add warp charge] to update the warp charge value
            Messenger.Instance.Subscribe<AddWarpChargeMessage>(this, (sender, message) =>
            {
                if (isServer)
                {
                    ++m_WarpCharge;

                    if (m_WarpCharge >= ChargesToSwitchTheme)
                    {
                        m_WarpCharge = 0;

                        var themeIndex = 0;
                        while (true)
                        {
                            // Choose a randomm theme
                            themeIndex = RandomUtility.RandomArrayIndex(ChunkDatabase.Themes, theme => theme.Likelihood);

                            // Prevent same theme to appear twice in a row
                            if (ChunkDatabase.Themes[themeIndex] != CurrentTheme)
                                break;
                        }

                        // Switch to the same theme on all clients using an RPC
                        RpcSwitchTheme((byte)themeIndex);
                    }
                }
            });

            // Set first theme when the game starts
            CurrentTheme = ChunkDatabase.Themes[0];
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// The <see cref="m_WarpCharge"/> [SyncVar] hook.
        /// </summary>
        /// <param name="value">The value of <see cref="m_WarpCharge"/> on the server.</param>
        private void WarpChargeHook(byte value)
        {
            m_WarpCharge = value;

            // Update UI
            UI.WarpCharge.value = m_WarpCharge / (float)ChargesToSwitchTheme;
        }

        /// <summary>
        /// Makes the manager switch to the specified theme.
        /// </summary>
        /// <param name="themeIndex">Index of the theme in the chunk database.</param>
        [ClientRpc]
        private void RpcSwitchTheme(byte themeIndex)
        {
            CurrentTheme = ChunkDatabase.Themes[themeIndex];

            // Send [theme switched]
            Messenger.Instance.Send(this, new ThemeSwitchedMessage() { Theme = CurrentTheme });
        }
    }
}