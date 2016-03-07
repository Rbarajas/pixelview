using PixelView.Time_.Gameplay;
using System;
using UnityEditor;
using UnityEngine;

namespace PixelView.Time_.Data
{
    /// <summary>
    /// The item database editor
    /// </summary>
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var itemDatabase = target as ItemDatabase;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("New", EditorStyles.miniButtonLeft))
            {
                var items = new ItemData[itemDatabase.Items.Length + 1];
                Array.Copy(itemDatabase.Items, items, itemDatabase.Items.Length);
                items[items.Length - 1] = new ItemData();
                itemDatabase.Items = items;
            }

            if (GUILayout.Button("Show All", EditorStyles.miniButtonMid))
            {
                foreach (var item in itemDatabase.Items)
                    item.EditorInternal.Foldout = true;
            }

            if (GUILayout.Button("Hide All", EditorStyles.miniButtonMid))
            {
                foreach (var item in itemDatabase.Items)
                    item.EditorInternal.Foldout = false;
            }

            if (GUILayout.Button("Sort", EditorStyles.miniButtonRight))
            {
                Array.Sort(itemDatabase.Items, (item, other) => string.Compare(item.Name, other.Name));
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (itemDatabase.Items != null)
            {
                for (var i = 0; i < itemDatabase.Items.Length; ++i)
                {
                    DrawItem(itemDatabase, i);
                }
            }

            EditorUtility.SetDirty(itemDatabase);
        }

        private void DrawItem(ItemDatabase itemDatabase, int itemIndex)
        {
            var item = itemDatabase.Items[itemIndex];

            item.ItemId = itemIndex + 1;

            var name = !string.IsNullOrEmpty(item.Name) ? item.Name : "Item " + itemIndex;

            if (item.EditorInternal.Foldout = EditorGUILayout.Foldout(item.EditorInternal.Foldout, name))
            {
                ++EditorGUI.indentLevel;

                EditorGUILayout.LabelField("Item Id", item.ItemId.ToString("D4"));

                item.Name = EditorGUILayout.TextField("Name", item.Name);

                EditorGUILayout.BeginHorizontal();

                item.Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", item.Prefab, typeof(GameObject), false);

                if (item.Prefab != null)
                {
                    var assetPreview = AssetPreview.GetAssetPreview(item.Prefab);

                    if (assetPreview != null)
                    {
                        var position = EditorGUILayout.GetControlRect(GUILayout.Width(64), GUILayout.Height(64));
                        EditorGUI.DrawPreviewTexture(position, assetPreview);
                    }
                }

                EditorGUILayout.EndHorizontal();

                item.Likelihood = EditorGUILayout.IntField("Likelihood", item.Likelihood);

                item.ItemApplyMode = (ItemApplyMode)EditorGUILayout.EnumPopup("Apply Mode", item.ItemApplyMode);

                if ((item.ItemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", item.ItemType)) == ItemType.AddEffect)
                {
                    item.EffectType = (EffectType)EditorGUILayout.EnumPopup("Effect Type", item.EffectType);

                    item.EffectDuration = EditorGUILayout.FloatField("Effect Duration", item.EffectDuration);
                }

                item.Color = EditorGUILayout.ColorField("Color", item.Color);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Insert", EditorStyles.miniButtonLeft))
                {
                    var tmp = new ItemData[itemDatabase.Items.Length + 1];
                    Array.Copy(itemDatabase.Items, tmp, itemIndex);
                    Array.Copy(itemDatabase.Items, itemIndex, tmp, itemIndex + 1, itemDatabase.Items.Length - itemIndex);
                    tmp[itemIndex] = new ItemData();
                    itemDatabase.Items = tmp;
                }

                if (GUILayout.Button("Delete", EditorStyles.miniButtonMid))
                {
                    var tmp = new ItemData[itemDatabase.Items.Length - 1];
                    Array.Copy(itemDatabase.Items, tmp, itemIndex);
                    Array.Copy(itemDatabase.Items, itemIndex + 1, tmp, itemIndex, itemDatabase.Items.Length - 1 - itemIndex);
                    itemDatabase.Items = tmp;
                }

                GUI.enabled = itemIndex > 0;
                if (GUILayout.Button("Move Up", EditorStyles.miniButtonMid))
                {
                    var tmp = itemDatabase.Items[itemIndex - 1];
                    itemDatabase.Items[itemIndex - 1] = itemDatabase.Items[itemIndex];
                    itemDatabase.Items[itemIndex] = tmp;
                }

                GUI.enabled = itemIndex < itemDatabase.Items.Length - 1;
                if (GUILayout.Button("Move Down", EditorStyles.miniButtonRight))
                {
                    var tmp = itemDatabase.Items[itemIndex + 1];
                    itemDatabase.Items[itemIndex + 1] = itemDatabase.Items[itemIndex];
                    itemDatabase.Items[itemIndex] = tmp;
                }

                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();

                --EditorGUI.indentLevel;

                EditorGUILayout.Space();
            }
        }
    }
}