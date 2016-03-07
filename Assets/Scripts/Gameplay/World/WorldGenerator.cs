using PixelView.Time_.Data;
using PixelView.Time_.Gameplay.Player;
using PixelView.Time_.Utility;
using UnityEngine;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.World
{
    /// <summary>
    /// Generates the world chunks
    /// </summary>
    [RequireComponent(typeof(SyncedPosition))]
    [RequireComponent(typeof(ThemeManager))]
    public class WorldGenerator : NetworkBehaviour
    {
        /// <summary>
        /// The item database
        /// </summary>
        public ItemDatabase ItemDatabase;

        /// <summary>
        /// The horizon is the minimum distance between [distance travelled] and the last spawned
        /// chunk. This value determines when new chunks are spawned.
        /// </summary>
        public float Horizon = 100;

        /// <summary>
        /// The warm up distance
        /// </summary>
        public float WarmUpDistance = 100;


        // Reference to the required [synced position] component
        private SyncedPosition m_SyncedPosition;

        // Reference to the required [theme manager] component
        private ThemeManager m_ThemeManager;

        // The position where the next chunk will be spawned
        private Vector3 m_SpawnPosition;

        // Is the [panic] effect active?
        private bool m_Panic;

        /// <summary>
        /// Called when [awake].
        /// </summary>
        private void Awake()
        {
            m_SyncedPosition = GetComponent<SyncedPosition>();
            m_ThemeManager = GetComponent<ThemeManager>();

            // Subscribe to [player added] to get the synced rng seed
            Messenger.Instance.Subscribe<PlayerAddedMessage>(this, (sender, message) =>
            {
                var playerInfo = message.Player.GetComponent<PlayerInfo>();

                // Always get the seed from the first player
                if (playerInfo.PlayerIndex == 0)
                {
                    Random.seed = playerInfo.WorldSeed;
                }
            });

            // Subscribe to [effect changed] to handle [panic]
            Messenger.Instance.Subscribe<EffectChangedMessage>(this, (sender, message) =>
            {
                if (message.Effect == EffectType.Panic)
                {
                    m_Panic = message.Duration > 0;
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
            // Spawn chunks as needed based on the [distance travelled] so far
            while (m_SyncedPosition.DistanceTravelled + Horizon > m_SpawnPosition.z)
            {
                SpawnChunk();
            }
        }

        /// <summary>
        /// Spawns a world chunk
        /// </summary>
        private void SpawnChunk()
        {
            // Pick a random chunk
            var chunkData = PickChunk();

            // Instantiate the new chunk and parent it to this object
            var chunk = Instantiate(chunkData.Prefab, m_SpawnPosition, Quaternion.identity) as GameObject;
            chunk.transform.SetParent(transform, true);

            var spinner = chunk.GetComponentInChildren<Spinner>();
            if (spinner != null)
            {
                // Set a random rotation
                spinner.transform.rotation = Quaternion.Euler(0, 0, (float)Random.Range(0, 8) * 360 / 8);

                // Give a rotation speed to the chunk
                if (Random.value < 0.5f)
                    spinner.RotationSpeed = new Vector3(0, 0, 30 + Random.Range(0, 2) * 30);

                // Spawn items
                if (Random.value < chunkData.ItemChance)
                {
                    // Pick an item type from the database
                    var itemData = RandomUtility.RandomArrayElement(ItemDatabase.Items, item => item.Likelihood);

                    foreach (Transform child in spinner.transform)
                    {
                        // Child objects tagged as "Item" are used to mark the positions where items
                        // should be spawned
                        if (child.CompareTag("Item"))
                        {
                            var item = Instantiate(itemData.Prefab, Vector3.zero, Quaternion.identity) as GameObject;
                            item.transform.SetParent(child.transform, false);

                            item.GetComponent<Item>().ItemId = itemData.ItemId;
                        }
                    }
                }
            }

            // Use the renderer bounds to compute chunk's length
            // This solution works for now but may not be ideal in future
            var chunkLength = chunk.GetComponent<Renderer>().bounds.size.z;
            m_SpawnPosition += new Vector3(0, 0, chunkLength);
        }

        /// <summary>
        /// Chooses which chunk to spawn.
        /// </summary>
        /// <returns>The chosen <see cref="ChunkData"/>.</returns>
        private ChunkData PickChunk()
        {
            var theme = m_ThemeManager.CurrentTheme;

            if (m_SyncedPosition.DistanceTravelled < WarmUpDistance)
            {
                // While [distance travelled] is less than [warm up distance] spawn the first chunk
                // of the theme which is assumed to be the easiest one
                return theme.Chunks[0];
            }

            // Choose which chunk list to use
            var chunks = m_Panic ? theme.PanicChunks : theme.Chunks;

            // And use it to pick a random chunk
            return RandomUtility.RandomArrayElement(chunks, chunk => chunk.Likelihood);
        }
    }
}