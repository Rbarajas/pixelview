using PixelView.Time_.Data;
using UnityEngine.Networking;
using UnityToolkit.Messaging;

namespace PixelView.Time_.Gameplay.Player
{
    /// <summary>
    /// Message sent when a player collects an item
    /// </summary>
    public struct ItemCollectedMessage
    {
        /// <summary>
        /// The <see cref="PlayerCollector"/> component of the player that has collected the item
        /// </summary>
        public PlayerCollector Collector;

        /// <summary>
        /// A number indentifying the type of the item that was collected
        /// </summary>
        public int ItemId;
    }


    /// <summary>
    /// Message sent to increase the warp charge
    /// </summary>
    public struct AddWarpChargeMessage
    {
    }


    /// <summary>
    /// Handles items collection
    /// </summary>
    public class PlayerCollector : NetworkBehaviour
    {
        /// <summary>
        /// The item database
        /// </summary>
        public ItemDatabase ItemDatabase;


        /// <summary>
        /// Called when the player collects an item.
        /// </summary>
        /// <param name="itemId">The identifier of the collected item.</param>
        public void Collect(int itemId)
        {
            if (isLocalPlayer)
                Messenger.Instance.Send(this, new ItemCollectedMessage() { Collector = this, ItemId = itemId });

            // Inform the server
            CmdCollect((byte)itemId);
        }

        /// <summary>
        /// Informs the server that this player collected an item.
        /// </summary>
        /// <param name="itemId">The identifier of the collected item.</param>
        [Command]
        private void CmdCollect(byte itemId)
        {
            // Inform all clients
            RpcCollect(itemId);
        }

        /// <summary>
        /// Informs all clients that this player collected an item.
        /// </summary>
        /// <param name="itemId">The identifier of the collected item.</param>
        [ClientRpc]
        private void RpcCollect(byte itemId)
        {
            if (!isLocalPlayer)
                // Send [item collected]
                Messenger.Instance.Send(this, new ItemCollectedMessage() { Collector = this, ItemId = itemId });

            // Fetch the item from the database
            var item = ItemDatabase.FindItemById(itemId);

            // Determine whether the item should have an effect on this client
            if ((isLocalPlayer && item.ItemApplyMode == ItemApplyMode.OthersOnly) ||
                (!isLocalPlayer && item.ItemApplyMode == ItemApplyMode.LocalOnly))
                return;

            // Apply the item behaviour
            ApplyItem(item);
        }

        /// <summary>
        /// Applies an item's behaviour.
        /// </summary>
        /// <param name="item">The item whose bahaviour has to be applied.</param>
        private void ApplyItem(ItemData item)
        {
            // The [item type] is used to determine which behaviour to apply
            switch (item.ItemType)
            {
                // An item that triggers an effect on the [effect manager]
                case ItemType.AddEffect:
                    // Send [effect changed]
                    Messenger.Instance.Send(this, new EffectChangedMessage() { Effect = item.EffectType, Duration = item.EffectDuration });
                    break;

                // An item that grants an [extra life]
                case ItemType.ExtraLife:
                    GetComponent<PlayerLife>().Add(1);
                    break;

                // An item that adds a [warp charge]
                case ItemType.WarpCharge:
                    // Send [add warp charge]
                    Messenger.Instance.Send(this, new AddWarpChargeMessage());
                    break;
            }
        }
    }
}