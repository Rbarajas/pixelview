using System;
using UnityEngine;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.World
{
    /// <summary>
    /// Manages uniform params of the Curvature shader.
    /// </summary>
    [RequireComponent(typeof(SyncedPosition))]
    public class CurvatureShaderManager : MonoBehaviour
    {
        /// <summary>
        /// Parameters relative to the curvature effect
        /// </summary>
        [Serializable]
        public class CurvatureParams
        {
            /// <summary>
            /// The distance from the camera where the effect begins
            /// </summary>
            public float Distance = 10;

            /// <summary>
            /// The maximum curvature
            /// </summary>
            public float MaxCurvature = 0.005f;

            /// <summary>
            /// The frequency of change of the horizontal curvature
            /// </summary>
            public float HorizontalChangeFrequency = 0.001f;

            /// <summary>
            /// The frequency of change of the vertical curvature
            /// </summary>
            public float VerticalChangeFrequency = 0.002f;
        }


        /// <summary>
        /// Parameters relative to the fog effect
        /// </summary>
        [Serializable]
        public class FogParams
        {
            /// <summary>
            /// The distance from the camera where the fog begins
            /// </summary>
            public float FogStart;

            /// <summary>
            /// The distance from the camera where the fog ends
            /// </summary>
            public float FogEnd = 100;
        }


        /// <summary>
        /// The curvature parameters
        /// </summary>
        public CurvatureParams Curvature;

        /// <summary>
        /// The fog parameters
        /// </summary>
        public FogParams Fog;

        /// <summary>
        /// The fog parameters to use when the [blind] effect is active
        /// </summary>
        public FogParams BlindModeFog;


        // Reference to the required [synced position] component
        private SyncedPosition m_SyncedPosition;


        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {
            m_SyncedPosition = GetComponent<SyncedPosition>();

            Shader.SetGlobalFloat("_FogMin", Fog.FogStart);
            Shader.SetGlobalFloat("_FogMax", Fog.FogEnd);

            // Subscribe to [effect changed] to handle [blind]
            Messenger.Instance.Subscribe<EffectChangedMessage>(this, (sender, message) =>
            {
                if (message.Effect == EffectType.Blind)
                {
                    // Choose fog params based on whether the [blind] effect is active or not
                    var fogMin = message.Duration > 0 ? BlindModeFog.FogStart : Fog.FogStart;
                    var fogMax = message.Duration > 0 ? BlindModeFog.FogEnd : Fog.FogEnd;

                    // Apply the params to the shader
                    Shader.SetGlobalFloat("_FogMin", fogMin);
                    Shader.SetGlobalFloat("_FogMax", fogMax);
                }
            });
        }

        private void OnDestroy()
        {
            Messenger.Instance.Unsubscribe(this);
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        private void Update()
        {
            // Compute vertical and horizontal curvature factors
            var curvH = Mathf.Cos(Mathf.PI * 2 * Curvature.HorizontalChangeFrequency * m_SyncedPosition.DistanceTravelled);
            var curvV = Mathf.Sin(Mathf.PI * 2 * Curvature.VerticalChangeFrequency * m_SyncedPosition.DistanceTravelled);

            // Set curvature shader params
            Shader.SetGlobalFloat("_Distance", Curvature.Distance);
            Shader.SetGlobalFloat("_MaxCurvature", Curvature.MaxCurvature);
            Shader.SetGlobalFloat("_CurvatureH", curvH);
            Shader.SetGlobalFloat("_CurvatureV", curvV);
        }
    }
}