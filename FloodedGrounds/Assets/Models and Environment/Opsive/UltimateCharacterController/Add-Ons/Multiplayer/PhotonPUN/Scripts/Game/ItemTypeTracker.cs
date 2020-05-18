/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using Opsive.UltimateCharacterController.Inventory;
using System.Collections.Generic;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game
{
    /// <summary>
    /// Maps the Item Type ID to the Item Type over the network.
    /// </summary>
    public class ItemTypeTracker : MonoBehaviour
    {
        private static ItemTypeTracker s_Instance;
        private static ItemTypeTracker Instance
        {
            get
            {
                if (!s_Initialized) {
                    s_Instance = new GameObject("ItemType Tracker").AddComponent<ItemTypeTracker>();
                    s_Initialized = true;
                }
                return s_Instance;
            }
        }
        private static bool s_Initialized;

        [Tooltip("A reference to all of the available ItemTypes.")]
        [SerializeField] protected ItemCollection m_ItemCollection;

        public ItemCollection ItemCollection { get { return m_ItemCollection; } set { m_ItemCollection = value; } }

        private Dictionary<int, ItemType> m_IDItemTypeMap = new Dictionary<int, ItemType>();

        /// <summary>
        /// The object has been enabled.
        /// </summary>
        private void OnEnable()
        {
            // The object may have been enabled outside of the scene unloading.
            if (s_Instance == null) {
                s_Instance = this;
                s_Initialized = true;
                SceneManager.sceneUnloaded -= SceneUnloaded;
            }
        }

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            for (int i = 0; i < m_ItemCollection.ItemTypes.Length; ++i) {
                m_IDItemTypeMap.Add(m_ItemCollection.ItemTypes[i].ID, m_ItemCollection.ItemTypes[i]);
            }
        }

        /// <summary>
        /// An ItemType has been picked up within the inventory.
        /// </summary>
        /// <param name="itemType">The ItemType that has been equipped.</param>
        /// <param name="amount">The amount of ItemType picked up.</param>
        /// <param name="immediatePickup">Was the item be picked up immediately?</param>
        /// <param name="forceEquip">Should the item be force equipped?</param>
        private void OnPickupItemType(ItemType itemType, float amount, bool immediatePickup, bool forceEquip)
        {
            if (!m_IDItemTypeMap.ContainsKey(itemType.ID)) {
                m_IDItemTypeMap.Add(itemType.ID, itemType);
            }
        }

        /// <summary>
        /// Returns the ItemType that belongs to the specified ID.
        /// </summary>
        /// <param name="id">The ID of the ItemType to retrieve.</param>
        /// <returns>The ItemType that belongs to the specified ID.</returns>
        public static ItemType GetItemType(int id)
        {
            return Instance.GetItemTypeInternal(id);
        }
        
        /// <summary>
        /// Internal method which returns the ItemType that belongs to the specified ID.
        /// </summary>
        /// <param name="id">The ID of the ItemType to retrieve.</param>
        /// <returns>The ItemType that belongs to the specified ID.</returns>
        private ItemType GetItemTypeInternal(int id)
        {
            ItemType itemType;
            if (m_IDItemTypeMap.TryGetValue(id, out itemType)) {
                return itemType;
            }
            return null;
        }

        /// <summary>
        /// Reset the initialized variable when the scene is no longer loaded.
        /// </summary>
        /// <param name="scene">The scene that was unloaded.</param>
        private void SceneUnloaded(Scene scene)
        {
            s_Initialized = false;
            s_Instance = null;
            SceneManager.sceneUnloaded -= SceneUnloaded;
        }

        /// <summary>
        /// The object has been disabled.
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
        }
    }
}