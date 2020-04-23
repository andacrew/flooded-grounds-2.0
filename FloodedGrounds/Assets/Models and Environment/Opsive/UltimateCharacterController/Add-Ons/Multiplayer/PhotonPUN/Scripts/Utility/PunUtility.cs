/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;
using System.Collections.Generic;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Utility
{
    /// <summary>
    /// Small utility methods that interact with PUN.
    /// </summary>
    public static class PunUtility
    {
        private static Dictionary<GameObject, Dictionary<int, ObjectIdentifier>> s_IDObjectIDMap = new Dictionary<GameObject, Dictionary<int, ObjectIdentifier>>();
        private static Dictionary<int, ObjectIdentifier> s_SceneIDMap = new Dictionary<int, ObjectIdentifier>();

        /// <summary>
        /// Registers the Object Identifier within the scene ID map.
        /// </summary>
        /// <param name="sceneObjectIdentifier">The Object Identifier that should be registered.</param>
        public static void RegisterSceneObjectIdentifier(ObjectIdentifier sceneObjectIdentifier)
        {
            if (s_SceneIDMap.ContainsKey(sceneObjectIdentifier.ID)) {
                Debug.LogError("Error: The scene object ID " + sceneObjectIdentifier.ID + " already exists. In order to correct run Scene Setup again on this scene.");
                return;
            }
            s_SceneIDMap.Add(sceneObjectIdentifier.ID, sceneObjectIdentifier);
        }

        /// <summary>
        /// Returns the PUN-friendly ID for the specified GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the ID of.</param>
        /// <param name="itemSlotID">If the object is an item then return the slot ID of the item.</param>
        /// <returns>The ID for the specified GameObject.</returns>
        public static int GetID(GameObject gameObject, out int itemSlotID)
        {
            itemSlotID = -1;
            if (gameObject == null) {
                return -1;
            }

            var id = -1;
            var photonView = gameObject.GetCachedComponent<PhotonView>();
            if (photonView != null) {
                id = photonView.ViewID;
            } else {
                // Try to get the ObjectIdentifier.
                var objectIdentifier = gameObject.GetCachedComponent<ObjectIdentifier>();
                if (objectIdentifier != null) {
                    id = objectIdentifier.ID;
                } else {
                    // The object may be an item.
                    var inventory = gameObject.GetCachedParentComponent<InventoryBase>();
                    if (inventory != null) {
                        for (int i = 0; i < inventory.SlotCount; ++i) {
                            var item = inventory.GetItem(i);
                            if (item == null) {
                                continue;
                            }
                            var visibleObject = item.ActivePerspectiveItem.GetVisibleObject();
                            if (gameObject == visibleObject) {
                                id = item.ItemType.ID;
                                itemSlotID = item.SlotID;
                                break;
                            }
                        }

                        // The item may be a holstered item.
                        if (id == -1) {
                            var allItems = inventory.GetAllItems();
                            for (int i = 0; i < allItems.Count; ++i) {
                                var visibleObject = allItems[i].ActivePerspectiveItem.GetVisibleObject();
                                if (gameObject == visibleObject) {
                                    id = allItems[i].ItemType.ID;
                                    itemSlotID = allItems[i].SlotID;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (id == -1) {
                Debug.LogWarning("Error: The object " + gameObject.name + " does not contain a PhotonView or ObjectIdentifier. It will not be able to be sent over the network.");
            }

            return id;
        }

        /// <summary>
        /// Retrieves the GameObject with the specified ID.
        /// </summary>
        /// <param name="parent">The parent GameObject to the object with the specified ID.</param>
        /// <param name="id">The ID to search for.</param>
        /// <param name="itemSlotID">If the object is an item then the slot ID will specify which slot the item is from.</param>
        /// <returns>The GameObject with the specified ID. Can be null.</returns>
        public static GameObject RetrieveGameObject(GameObject parent, int id, int itemSlotID)
        {
            if (id == -1) {
                return null;
            }

            // The ID can be a PhotonView, ObjectIdentifier, or Item ID. Search for the ObjectIdentifier first and then the PhotonView.
            GameObject gameObject = null;
            if (itemSlotID == -1) {
                Dictionary<int, ObjectIdentifier> idObjectIDMap;
                ObjectIdentifier objectIdentifier = null;
                if (parent == null) {
                    idObjectIDMap = s_SceneIDMap;
                } else {
                    if (!s_IDObjectIDMap.TryGetValue(parent, out idObjectIDMap)) {
                        idObjectIDMap = new Dictionary<int, ObjectIdentifier>();
                        s_IDObjectIDMap.Add(parent, idObjectIDMap);
                    }
                }
                if (!idObjectIDMap.TryGetValue(id, out objectIdentifier)) {
                    // The ID doesn't exist in the cache. Try to find the object.
                    var objectIdentifiers = parent == null ? GameObject.FindObjectsOfType<ObjectIdentifier>() : parent.GetComponentsInChildren<ObjectIdentifier>();
                    if (objectIdentifiers != null) {
                        for (int i = 0; i < objectIdentifiers.Length; ++i) {
                            if (objectIdentifiers[i].ID == id) {
                                objectIdentifier = objectIdentifiers[i];
                                break;
                            }
                        }
                    }
                    idObjectIDMap.Add(id, objectIdentifier);
                }
                if (objectIdentifier != null) {
                    gameObject = objectIdentifier.gameObject;
                } else {
                    // The ID wasn't from a ObjectIdentifier. Search for the PhotonView.
                    var hitPhotonView = PhotonNetwork.GetPhotonView(id);
                    if (hitPhotonView == null) {
                        Debug.LogError("Error: Unable to find the object with ID " + id);
                        return null;
                    }
                    gameObject = hitPhotonView.gameObject;
                }
            } else { // The ID is an item.
                if (parent == null) {
                    Debug.LogError("Error: The parent must exist in order to retrieve the item ID.");
                    return null;
                }

                var itemType = Game.ItemTypeTracker.GetItemType(id);
                if (itemType == null) {
                    Debug.LogError("Error: The ItemType with id " + id + " does not exist.");
                    return null;
                }

                var inventory = parent.GetCachedParentComponent<InventoryBase>();
                if (inventory == null) {
                    Debug.LogError("Error: The parent does not contain an inventory.");
                    return null;
                }

                var item = inventory.GetItem(itemSlotID, itemType);
                // The item may not exist if it was removed shortly after it was hit on sending client.
                if (item == null) {
                    return null;
                }

                return item.ActivePerspectiveItem.GetVisibleObject();
            }

            return gameObject;
        }

        /// <summary>
        /// Unregisters the Object Identifier within the scene ID map.
        /// </summary>
        /// <param name="sceneObjectIdentifier">The Object Identifier that should be unregistered.</param>
        public static void UnregisterSceneObjectIdentifier(ObjectIdentifier sceneObjectIdentifier)
        {
            s_SceneIDMap.Remove(sceneObjectIdentifier.ID);
        }
    }
}