/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Items;
using Opsive.UltimateCharacterController.Utility;
using Photon.Realtime;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game
{
    /// <summary>
    /// Adds all runtime pickups to the character after they joined the room.
    /// </summary>
    public class RuntimePickups : MonoBehaviour
    {
        [Tooltip("An array of items that can be picked up at runtime. Any runtime pickup item must be specified within this array.")]
        [SerializeField] protected Item[] m_RuntimeItems;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }

        /// <summary>
        /// A player has entered the room. Ensure the joining player is in sync with the current game state.
        /// </summary>
        /// <param name="player">The Photon Player that entered the room.</param>
        /// <param name="character">The character that the player controls.</param>
        private void OnPlayerEnteredRoom(Player player, GameObject character)
        {
            var inventory = character.GetCachedComponent<InventoryBase>();
            if (inventory == null) {
                return;
            }

            var itemPlacement = character.GetComponentInChildren<ItemPlacement>(true);
            if (itemPlacement == null) {
                return;
            }

            // Add all runtime pickups to the character as soon as the player joins. This ensures the joining character can equip any already picked up items.
            for (int i = 0; i < m_RuntimeItems.Length; ++i) {
                if (m_RuntimeItems[i] == null || inventory.HasItem(m_RuntimeItems[i])) {
                    continue;
                }

                var itemGameObject = ObjectPool.Instantiate(m_RuntimeItems[i], Vector3.zero, Quaternion.identity, itemPlacement.transform);
                itemGameObject.name = m_RuntimeItems[i].name;
                itemGameObject.transform.localPosition = Vector3.zero;
                itemGameObject.transform.localRotation = Quaternion.identity;
                inventory.AddItem(itemGameObject.GetComponent<Item>(), false);
            }
        }

        /// <summary>
        /// The object has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }
    }
}