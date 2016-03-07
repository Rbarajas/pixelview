using PixelView.Time_.Gameplay;
using System;
using System.Linq;
using UnityEngine;

namespace PixelView.Time_.Data
{
    /// <summary>
    /// The item apply mode
    /// </summary>
    public enum ItemApplyMode
    {
        /// <summary>
        /// The local only
        /// </summary>
        LocalOnly,

        /// <summary>
        /// The others only
        /// </summary>
        OthersOnly,

        /// <summary>
        /// The everybody
        /// </summary>
        Everybody
    }


    /// <summary>
    /// The item type
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// The add effect
        /// </summary>
        AddEffect,

        /// <summary>
        /// The extra life
        /// </summary>
        ExtraLife,

        /// <summary>
        /// The warp charge
        /// </summary>
        WarpCharge
    }


    /// <summary>
    /// The item data
    /// </summary>
    [Serializable]
    public class ItemData
    {
#if UNITY_EDITOR
        [Serializable]
        public class EditorInternal_
        {
            public bool Foldout = true;
        }


        public EditorInternal_ EditorInternal;
#endif

        /// <summary>
        /// The item identifier
        /// </summary>
        public int ItemId;

        /// <summary>
        /// The name
        /// </summary>
        public string Name = "New Item";

        /// <summary>
        /// The prefab
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// The likelihood
        /// </summary>
        public int Likelihood = 1;

        /// <summary>
        /// The item apply mode
        /// </summary>
        public ItemApplyMode ItemApplyMode = ItemApplyMode.Everybody;

        /// <summary>
        /// The item type
        /// </summary>
        public ItemType ItemType;

        /// <summary>
        /// The effect type
        /// </summary>
        public EffectType EffectType;

        /// <summary>
        /// The effect duration
        /// </summary>
        public float EffectDuration = 10;

        /// <summary>
        /// The color
        /// </summary>
        public Color Color = new Color(1, 1, 1, 0.2f);
    }


    /// <summary>
    /// The item database
    /// </summary>
    [CreateAssetMenu(fileName = "Item Database", menuName = "Game Data/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        /// <summary>
        /// The items
        /// </summary>
        public ItemData[] Items = new ItemData[0];


        /// <summary>
        /// Finds an item by its identifier.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>The item.</returns>
        public ItemData FindItemById(int itemId)
        {
            return Items.FirstOrDefault(item => item.ItemId == itemId);
        }
    }
}