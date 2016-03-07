using System;
using UnityEngine;

namespace PixelView.Time_.Data
{
    /// <summary>
    /// The chunk data
    /// </summary>
    [Serializable]
    public class ChunkData
    {
        /// <summary>
        /// The prefab
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// The likelihood
        /// </summary>
        public int Likelihood = 1;

        /// <summary>
        /// The item chance
        /// </summary>
        [Range(0, 1)]
        public float ItemChance = 1;
    }


    /// <summary>
    /// The theme data
    /// </summary>
    [Serializable]
    public class ThemeData
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Name;

        /// <summary>
        /// The likelihood
        /// </summary>
        public int Likelihood = 1;

        /// <summary>
        /// The chunks
        /// </summary>
        public ChunkData[] Chunks;

        /// <summary>
        /// The panic chunks
        /// </summary>
        public ChunkData[] PanicChunks;
    }


    /// <summary>
    /// The chunk database
    /// </summary>
    [CreateAssetMenu(fileName = "Chunk Database", menuName = "Game Data/Chunk Database")]
    public class ChunkDatabase : ScriptableObject
    {
        /// <summary>
        /// The themes
        /// </summary>
        public ThemeData[] Themes;
    }
}