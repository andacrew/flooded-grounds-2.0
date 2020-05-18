/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Items.Actions;
using Opsive.UltimateCharacterController.Networking.Character;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Character
{
    /// <summary>
    /// The PunCharacter component manages the RPCs and state of the character on the Photon network.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PunCharacter : MonoBehaviourPunCallbacks, INetworkCharacter
    {
        private GameObject m_GameObject;
        private UltimateCharacterLocomotion m_CharacterLocomotion;
        private InventoryBase m_Inventory;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_GameObject = gameObject;
            m_CharacterLocomotion = m_GameObject.GetCachedComponent<UltimateCharacterLocomotion>();
            m_Inventory = m_GameObject.GetCachedComponent<InventoryBase>();
        }

        /// <summary>
        /// Registers for any interested events.
        /// </summary>
        private void Start()
        {
            if (photonView.IsMine) {
                EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
                EventHandler.RegisterEvent<Ability, bool>(m_GameObject, "OnCharacterAbilityActive", OnAbilityActive);
                EventHandler.RegisterEvent<ItemAbility, bool>(m_GameObject, "OnCharacterItemAbilityActive", OnItemAbilityActive);
            } else {
                // Pickup isn't called on unequipped items.
                var allItems = m_Inventory.GetAllItems();
                for (int i = 0; i < allItems.Count; ++i) {
                    allItems[i].Pickup();
                }
            }
        }

        /// <summary>
        /// Loads the inventory's default loadout.
        /// </summary>
        public void LoadDefaultLoadout()
        {
            photonView.RPC("LoadDefaultLoadoutRPC", RpcTarget.Others);
        }

        /// <summary>
        /// Loads the inventory's default loadout on the network.
        /// </summary>
        [PunRPC]
        private void LoadDefaultLoadoutRPC()
        {
            m_Inventory.LoadDefaultLoadout();
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterSnapAnimator");
        }

        /// <summary>
        /// A player has entered the room. Ensure the joining player is in sync with the current game state.
        /// </summary>
        /// <param name="player">The Photon Player that entered the room.</param>
        /// <param name="character">The character that the player controls.</param>
        private void OnPlayerEnteredRoom(Player player, GameObject character)
        {
            if (m_Inventory != null) {
                // Notify the joining player of the ItemTypes that the player has within their inventory.
                var items = m_Inventory.GetAllItems();
                for (int i = 0; i < items.Count; ++i) {
                    var item = items[i];

                    photonView.RPC("PickupItemTypeRPC", player, item.ItemType.ID, m_Inventory.GetItemTypeCount(item.ItemType));

                    if (item.DropPrefab != null) {
                        // Usable Items have a separate ItemType count.
                        var itemActions = item.ItemActions;
                        for (int j = 0; j < itemActions.Length; ++j) {
                            var usableItem = itemActions[j] as IUsableItem;
                            if (usableItem == null) {
                                continue;
                            }

                            var consumableItemTypeCount = usableItem.GetConsumableItemTypeCount();
                            if (consumableItemTypeCount > 0 || consumableItemTypeCount == -1) { // -1 is used by the grenade to indicate that there is only one item.
                                photonView.RPC("PickupUsableItemActionRPC", player, item.ItemType.ID, item.SlotID, itemActions[j].ID,
                                                            m_Inventory.GetItemTypeCount(usableItem.GetConsumableItemType()), usableItem.GetConsumableItemTypeCount());
                            }
                        }
                    }
                }

                // Ensure the correct item is equipped in each slot.
                for (int i = 0; i < m_Inventory.SlotCount; ++i) {
                    var item = m_Inventory.GetItem(i);
                    if (item == null) {
                        continue;
                    }

                    photonView.RPC("EquipUnequipItemRPC", player, item.ItemType.ID, i, true);
                }
            }

            // ULTIMATE_CHARACTER_CONTROLLER_MULTIPLAYER will be defined, but it is required here to allow the addon to be compiled for the first time.
#if ULTIMATE_CHARACTER_CONTROLLER_MULTIPLAYER
            // The remote character should have the same abilities active.
            for (int i = 0; i < m_CharacterLocomotion.ActiveAbilityCount; ++i) {
                var activeAbility = m_CharacterLocomotion.ActiveAbilities[i];
                photonView.RPC("StartAbilityRPC", player, activeAbility.Index, activeAbility.GetNetworkStartData());
            }
            for (int i = 0; i < m_CharacterLocomotion.ActiveItemAbilityCount; ++i) {
                var activeItemAbility = m_CharacterLocomotion.ActiveItemAbilities[i];
                photonView.RPC("StartItemAbilityRPC", player, activeItemAbility.Index, activeItemAbility.GetNetworkStartData());
            }
#endif
        }

        /// <summary>
        /// The character's ability has been started or stopped.
        /// </summary>
        /// <param name="ability">The ability which was started or stopped.</param>
        /// <param name="active">True if the ability was started, false if it was stopped.</param>
        private void OnAbilityActive(Ability ability, bool active)
        {
            photonView.RPC("OnAbilityActiveRPC", RpcTarget.Others, ability.Index, active);
        }

        /// <summary>
        /// Activates or deactivates the ability on the network at the specified index.
        /// </summary>
        /// <param name="abilityIndex">The index of the ability.</param>
        /// <param name="active">Should the ability be activated?</param>
        [PunRPC]
        private void OnAbilityActiveRPC(int abilityIndex, bool active)
        {
            if (active) {
                m_CharacterLocomotion.TryStartAbility(m_CharacterLocomotion.Abilities[abilityIndex]);
            } else {
                m_CharacterLocomotion.TryStopAbility(m_CharacterLocomotion.Abilities[abilityIndex], true);
            }
        }

        /// <summary>
        /// Starts the ability on the remote player.
        /// </summary>
        /// <param name="abilityIndex">The index of the ability.</param>
        /// <param name="startData">Any data associated with the ability start.</param>
        [PunRPC]
        private void StartAbilityRPC(int abilityIndex, object[] startData)
        {
            var ability = m_CharacterLocomotion.Abilities[abilityIndex];
#if ULTIMATE_CHARACTER_CONTROLLER_MULTIPLAYER
            if (startData != null) {
                ability.SetNetworkStartData(startData);
            }
#endif
            m_CharacterLocomotion.TryStartAbility(ability, true, true);
        }

        /// <summary>
        /// Starts the item ability on the remote player.
        /// </summary>
        /// <param name="itemAbilityIndex">The index of the item ability.</param>
        /// <param name="startData">Any data associated with the item ability start.</param>
        [PunRPC]
        private void StartItemAbilityRPC(int itemAbilityIndex, object[] startData)
        {
            var itemAbility = m_CharacterLocomotion.ItemAbilities[itemAbilityIndex];
#if ULTIMATE_CHARACTER_CONTROLLER_MULTIPLAYER
            if (startData != null) {
                itemAbility.SetNetworkStartData(startData);
            }
#endif
            m_CharacterLocomotion.TryStartAbility(itemAbility, true, true);
        }

        /// <summary>
        /// The character's item ability has been started or stopped.
        /// </summary>
        /// <param name="itemAbility">The item ability which was started or stopped.</param>
        /// <param name="active">True if the ability was started, false if it was stopped.</param>
        private void OnItemAbilityActive(ItemAbility itemAbility, bool active)
        {
            photonView.RPC("OnItemAbilityActiveRPC", RpcTarget.Others, itemAbility.Index, active);
        }

        /// <summary>
        /// Activates or deactivates the item ability on the network at the specified index.
        /// </summary>
        /// <param name="itemAbilityIndex">The index of the item ability.</param>
        /// <param name="active">Should the ability be activated?</param>
        [PunRPC]
        private void OnItemAbilityActiveRPC(int itemAbilityIndex, bool active)
        {
            if (active) {
                m_CharacterLocomotion.TryStartAbility(m_CharacterLocomotion.ItemAbilities[itemAbilityIndex]);
            } else {
                m_CharacterLocomotion.TryStopAbility(m_CharacterLocomotion.ItemAbilities[itemAbilityIndex], true);
            }
        }

        /// <summary>
        /// Picks up the ItemType on the network.
        /// </summary>
        /// <param name="itemTypeID">The ID of the ItemType that should be equipped.</param>
        /// <param name="count">The number of ItemTypes to pickup.</param>
        [PunRPC]
        private void PickupItemTypeRPC(int itemTypeID, float count)
        {
            var itemType = ItemTypeTracker.GetItemType(itemTypeID);
            if (itemType == null) {
                return;
            }

            m_Inventory.PickupItemType(itemType, count, -1, false, false, false);
        }

        /// <summary>
        /// Picks up the IUsableItem ItemType on the network.
        /// </summary>
        /// <param name="itemTypeID">The ID of the ItemType that should be equipped.</param>
        /// <param name="slotID">The slot of the item being picked up.</param>
        /// <param name="itemActionID">The ID of the IUsableItem being picked up.</param>
        /// <param name="itemActionCount">The ConsumableItemType count within the inventory.</param>
        /// <param name="consumableItemTypeCount">The ConsumableItemType count loaded within the IUsableItem.</param>
        [PunRPC]
        private void PickupUsableItemActionRPC(int itemTypeID, int slotID, int itemActionID, float itemActionCount, float consumableItemTypeCount)
        {
            var itemType = ItemTypeTracker.GetItemType(itemTypeID);
            if (itemType == null) {
                return;
            }

            var item = m_Inventory.GetItem(slotID, itemType);
            if (item == null) {
                return;
            }

            var usableItemAction = item.GetItemAction(itemActionID) as IUsableItem;
            if (usableItemAction == null) {
                return;
            }

            // The IUsableItem has two counts: the first count is from the inventory, and the second count is set on the actual ItemAction.
            m_Inventory.PickupItemType(usableItemAction.GetConsumableItemType(), itemActionCount, -1, false, false, false);
            usableItemAction.SetConsumableItemTypeCount(consumableItemTypeCount);
        }

        /// <summary>
        /// Equips or unequips the item with the specified ItemType and slot.
        /// </summary>
        /// <param name="itemTypeID">The ID of the ItemType that should be equipped.</param>
        /// <param name="slotID">The slot of the item that should be equipped.</param>
        /// <param name="equip">Should the item be equipped? If false it will be unequipped.</param>
        public void EquipUnequipItem(int itemTypeID, int slotID, bool equip)
        {
            photonView.RPC("EquipUnequipItemRPC", RpcTarget.Others, itemTypeID, slotID, equip);
        }

        /// <summary>
        /// Equips or unequips the item on the network with the specified ItemType and slot.
        /// </summary>
        /// <param name="itemTypeID">The ID of the ItemType that should be equipped.</param>
        /// <param name="slotID">The slot of the item that should be equipped.</param>
        /// <param name="equip">Should the item be equipped? If false it will be unequipped.</param>
        [PunRPC]
        private void EquipUnequipItemRPC(int itemTypeID, int slotID, bool equip)
        {
            // The character has to be alive to equip.
            if (!m_CharacterLocomotion.Alive && equip) {
                return;
            }

            var itemType = ItemTypeTracker.GetItemType(itemTypeID);
            if (itemType == null) {
                return;
            }

            var item = m_Inventory.GetItem(slotID, itemType);
            if (item == null) {
                return;
            }

            if (equip) {
                m_Inventory.EquipItem(itemType, slotID, true);
            } else {
                m_Inventory.UnequipItem(itemType, slotID);
            }
        }

        /// <summary>
        /// The ItemType has been picked up.
        /// </summary>
        /// <param name="itemType">The ID of the ItemType that was picked up.</param>
        /// <param name="count">The number of ItemType picked up.</param>
        /// <param name="slotID">The ID of the slot which the item belongs to.</param>
        /// <param name="immediatePickup">Was the item be picked up immediately?</param>
        /// <param name="forcePickup">Should the item be force equipped?</param>
        public void ItemTypePickup(int itemTypeID, float count, int slotID, bool immediatePickup, bool forceEquip)
        {
            photonView.RPC("ItemTypePickupRPC", RpcTarget.Others, itemTypeID, count, slotID, immediatePickup, forceEquip);
        }

        /// <summary>
        /// The ItemType has been picked up on the network.
        /// </summary>
        /// <param name="itemType">The ID of the ItemType that was picked up.</param>
        /// <param name="count">The number of ItemType picked up.</param>
        /// <param name="slotID">The ID of the slot which the item belongs to.</param>
        /// <param name="immediatePickup">Was the item be picked up immediately?</param>
        /// <param name="forcePickup">Should the item be force equipped?</param>
        [PunRPC]
        private void ItemTypePickupRPC(int itemTypeID, float count, int slotID, bool immediatePickup, bool forceEquip)
        {
            var itemType = ItemTypeTracker.GetItemType(itemTypeID);
            if (itemType == null) {
                return;
            }

            m_Inventory.PickupItemType(itemType, count, slotID, immediatePickup, forceEquip);
        }

        /// <summary>
        /// Removes all of the items from the inventory.
        /// </summary>
        public void RemoveAllItems()
        {
            photonView.RPC("RemoveAllItemsRPC", RpcTarget.Others);
        }

        /// <summary>
        /// Removes all of the items from the inventory on the network.
        /// </summary>
        [PunRPC]
        private void RemoveAllItemsRPC()
        {
            m_Inventory.RemoveAllItems();
        }

#if ULTIMATE_CHARACTER_CONTROLLER_SHOOTER
        /// <summary>
        /// Fires the weapon.
        /// </summary>
        /// <param name="itemAction">The ItemAction that is being fired.</param>
        /// <param name="strength">(0 - 1) value indicating the amount of strength to apply to the shot.</param>
        public void Fire(ItemAction itemAction, float strength)
        {
            photonView.RPC("FireRPC", RpcTarget.Others, itemAction.Item.SlotID, itemAction.ID, strength);
        }

        /// <summary>
        /// Fires the weapon on the network.
        /// </summary>
        /// <param name="slotID">The slot of the ShootableWeapon being fired.</param>
        /// <param name="actionID">The ID of the ShootableWeapon being fired.</param>
        /// <param name="strength">(0 - 1) value indicating the amount of strength to apply to the shot.</param>
        [PunRPC]
        private void FireRPC(int slotID, int actionID, float strength)
        {
            var itemAction = GetItemAction(slotID, actionID) as ShootableWeapon;
            if (itemAction == null) {
                return;
            }
            itemAction.Fire(strength);
        }

        /// <summary>
        /// Returns the ItemAction with the specified slot and ID.
        /// </summary>
        /// <param name="slotID">The slot that the ItemAction belongs to.</param>
        /// <param name="actionID">The ID of the ItemAction being retrieved.</param>
        /// <returns>The ItemAction with the specified slot and ID</returns>
        private ItemAction GetItemAction(int slotID, int actionID)
        {
            var item = m_Inventory.GetItem(slotID);
            if (item == null) {
                return null;
            }
            return item.GetItemAction(actionID);
        }

        /// <summary>
        /// Starts to reload the item.
        /// </summary>
        /// <param name="itemAction">The ItemAction that is being reloaded.</param>
        public void StartItemReload(ItemAction itemAction)
        {
            photonView.RPC("StartItemReloadRPC", RpcTarget.Others, itemAction.Item.SlotID, itemAction.ID);
        }

        /// <summary>
        /// Starts to reload the item on the network.
        /// </summary>
        /// <param name="slotID">The slot of the ShootableWeapon being reloaded.</param>
        /// <param name="actionID">The ID of the ShootableWeapon being reloaded.</param>
        [PunRPC]
        private void StartItemReloadRPC(int slotID, int actionID)
        {
            var itemAction = GetItemAction(slotID, actionID);
            if (itemAction == null) {
                return;
            }
            (itemAction as ShootableWeapon).StartItemReload();
        }

        /// <summary>
        /// Reloads the item.
        /// </summary>
        /// <param name="itemAction">The ItemAction that is being reloaded.</param>
        /// <param name="fullClip">Should the full clip be force reloaded?</param>
        public void ReloadItem(ItemAction itemAction, bool fullClip)
        {
            photonView.RPC("ReloadItemRPC", RpcTarget.Others, itemAction.Item.SlotID, itemAction.ID, fullClip);
        }

        /// <summary>
        /// Reloads the item on the network.
        /// </summary>
        /// <param name="slotID">The slot of the ShootableWeapon being reloaded.</param>
        /// <param name="actionID">The ID of the ShootableWeapon being reloaded.</param>
        /// <param name="fullClip">Should the full clip be force reloaded?</param>
        [PunRPC]
        private void ReloadItemRPC(int slotID, int actionID, bool fullClip)
        {
            var itemAction = GetItemAction(slotID, actionID) as ShootableWeapon;
            if (itemAction == null) {
                return;
            }
            itemAction.ReloadItem(fullClip);
        }

        /// <summary>
        /// The item has finished reloading.
        /// </summary>
        /// <param name="itemAction">The ItemAction that is being reloaded.</param>
        /// <param name="success">Was the item reloaded successfully?</param>
        public void ItemReloadComplete(ItemAction itemAction, bool success)
        {
            photonView.RPC("ItemReloadCompleteRPC", RpcTarget.Others, itemAction.Item.SlotID, itemAction.ID, success);
        }

        /// <summary>
        /// The item has finished reloading on the network.
        /// </summary>
        /// <param name="slotID">The slot of the ShootableWeapon being reloaded.</param>
        /// <param name="actionID">The ID of the ShootableWeapon being reloaded.</param>
        /// <param name="success">Was the item reloaded successfully?</param>
        [PunRPC]
        private void ItemReloadCompleteRPC(int slotID, int actionID, bool success)
        {
            var itemAction = GetItemAction(slotID, actionID) as ShootableWeapon;
            if (itemAction == null) {
                return;
            }
            itemAction.ItemReloadComplete(success);
        }
#endif

#if ULTIMATE_CHARACTER_CONTROLLER_MELEE
        /// <summary>
        /// The melee weapon hit a collider.
        /// </summary>
        /// <param name="itemAction">The ItemAction that caused the collision.</param>
        /// <param name="hitboxIndex">The index of the hitbox that caused the collision.</param>
        /// <param name="raycastHit">The raycast that caused the collision.</param>
        /// <param name="hitGameObject">The GameObject that was hit.</param>
        /// <param name="hitCharacterLocomotion">The hit Ultimate Character Locomotion component.</param>
        public void MeleeHitCollider(ItemAction itemAction, int hitboxIndex, RaycastHit raycastHit, GameObject hitGameObject, UltimateCharacterLocomotion hitCharacterLocomotion)
        {
            var slotID = -1;
            var hitGameObjectID = Utility.PunUtility.GetID(hitGameObject, out slotID);
            var hitCharacterLocomotionViewID = -1;
            if (hitCharacterLocomotion != null) {
                var hitCharacterLocomotionView = hitCharacterLocomotion.gameObject.GetCachedComponent<PhotonView>();
                if (hitCharacterLocomotionView == null) {
                    Debug.LogError("Error: The character " + hitCharacterLocomotion.gameObject + " must have a PhotonView component added");
                    return;
                }
                hitCharacterLocomotionViewID = hitCharacterLocomotionView.ViewID;
            }

            photonView.RPC("MeleeHitColliderRPC", RpcTarget.Others, itemAction.Item.SlotID, itemAction.ID, hitboxIndex, raycastHit.point, raycastHit.normal, hitCharacterLocomotionViewID, hitGameObjectID, slotID);
        }

        /// <summary>
        /// The melee weapon hit a collider on the network.
        /// </summary>
        /// <param name="slotID">The slot of the MeleeWeapon that caused the collision.</param>
        /// <param name="actionID">The ID of the MeleeWeapon that caused the collision.</param>
        /// <param name="hitboxIndex">The index of the hitbox that caused the collision.</param>
        /// <param name="hitCharacterLocomotionViewID">The PhotonView ID of the hit Ultimate Character Locomotion component.</param>
        /// <param name="hitGameObjectID">The ID of the GameObject that was hit.</param>
        /// <param name="hitSlotID">If the hit GameObject is an item then the slot ID of the item will be specified.</param>
        [PunRPC]
        private void MeleeHitColliderRPC(int slotID, int actionID, int hitboxIndex, Vector3 raycastHitPoint, Vector3 raycastHitNormal, int hitCharacterLocomotionViewID, int hitGameObjectID, int hitSlotID)
        {
            var meleeWeapon = GetItemAction(slotID, actionID) as MeleeWeapon;
            if (meleeWeapon == null) {
                return;
            }

            // Retrieve the hit character before getting the hit GameObject so RetrieveGameObject will know the parent GameObject (if it exists).
            UltimateCharacterLocomotion characterLocomotion = null;
            if (hitCharacterLocomotionViewID != -1) {
                var character = PhotonNetwork.GetPhotonView(hitCharacterLocomotionViewID);
                if (character != null) {
                    characterLocomotion = character.gameObject.GetCachedComponent<UltimateCharacterLocomotion>();
                }
            }

            var hitGameObject = Utility.PunUtility.RetrieveGameObject((characterLocomotion != null ? characterLocomotion.gameObject : null), hitGameObjectID, hitSlotID);
            if (hitGameObject == null) {
                return;
            }
            var hitCollider = hitGameObject.GetCachedComponent<Collider>();
            if (hitCollider == null) {
                return;
            }

            // A RaycastHit cannot be sent over the network. Try to recreate it locally based on the position and normal values.
            RaycastHit raycastHit;
            var ray = new Ray(raycastHitPoint + raycastHitNormal * 1f, -raycastHitNormal);
            if (!hitCollider.Raycast(ray, out raycastHit, 2f)) {
                // The object has moved. Do a larger cast to try to find the object.
                if (!Physics.SphereCast(ray, 1f, 2f, 1 << hitGameObject.layer, QueryTriggerInteraction.Ignore)) {
                    // The object can't be found. Return.
                    return;
                }
            }

            var hitHealth = hitGameObject.GetCachedParentComponent<UltimateCharacterController.Traits.Health>();
            var hitbox = (meleeWeapon.ActivePerspectiveProperties as IMeleeWeaponPerspectiveProperties).Hitboxes[hitboxIndex];
            meleeWeapon.HitCollider(hitbox, raycastHit, hitGameObject, hitCollider, hitHealth, characterLocomotion);
        }
#endif

        /// <summary>
        /// Throws the throwable object.
        /// </summary>
        /// <param name="itemAction">The ThrowableItem that is performing the throw.</param>
        public void ThrowItem(ItemAction itemAction)
        {
            photonView.RPC("ThrowItemRPC", RpcTarget.Others, itemAction.Item.SlotID, itemAction.ID);
        }

        /// <summary>
        /// Throws the throwable object on the network.
        /// </summary>
        /// <param name="slotID">The slot of the ThrowableItem that is performing the throw.</param>
        /// <param name="actionID">The ID of the ThrowableItem that is performing the throw.</param>
        [PunRPC]
        private void ThrowItemRPC(int slotID, int actionID)
        {
            var itemAction = GetItemAction(slotID, actionID) as ThrowableItem;
            if (itemAction == null) {
                return;
            }

            itemAction.ThrowItem();
        }

        /// <summary>
        /// Enables the object mesh renderers for the ThrowableItem.
        /// </summary>
        /// <param name="itemAction">The ThrowableItem that is having the renderers enabled.</param>
        public void EnableThrowableObjectMeshRenderers(ItemAction itemAction)
        {
            photonView.RPC("EnableThrowableObjectMeshRenderersRPC", RpcTarget.Others, itemAction.Item.SlotID, itemAction.ID);
        }

        /// <summary>
        /// Enables the object mesh renderers for the ThrowableItem on the network.
        /// </summary>
        /// <param name="slotID">The slot of the ThrowableItem that is having the renderers enabled.</param>
        /// <param name="actionID">The ID of the ThrowableItem that is having the renderers enabled.</param>
        [PunRPC]
        private void EnableThrowableObjectMeshRenderersRPC(int slotID, int actionID)
        {
            var itemAction = GetItemAction(slotID, actionID) as ThrowableItem;
            if (itemAction == null) {
                return;
            }

            itemAction.EnableObjectMeshRenderers(true);
        }

        /// <summary>
        /// Pushes the target Rigidbody in the specified direction.
        /// </summary>
        /// <param name="targetRigidbody">The Rigidbody to push.</param>
        /// <param name="force">The amount of force to apply.</param>
        /// <param name="point">The point at which to apply the push force.</param>
        public void PushRigidbody(Rigidbody targetRigidbody, Vector3 force, Vector3 point)
        {
            var targetPhotonView = targetRigidbody.gameObject.GetCachedComponent<PhotonView>();
            if (targetPhotonView == null) {
                Debug.LogError("Error: The object" + targetRigidbody.gameObject + " must have a PhotonView component added");
                return;
            }

            photonView.RPC("PushRigidbodyRPC", RpcTarget.MasterClient, targetPhotonView.ViewID, force, point);
        }

        /// <summary>
        /// Pushes the target Rigidbody in the specified direction on the network.
        /// </summary>
        /// <param name="targetRigidbody">The Rigidbody to push.</param>
        /// <param name="force">The amount of force to apply.</param>
        /// <param name="point">The point at which to apply the push force.</param>
        [PunRPC]
        private void PushRigidbodyRPC(int rigidbodyPhotonViewID, Vector3 force, Vector3 point)
        {
            var targetRigidbodyPhotonView = PhotonNetwork.GetPhotonView(rigidbodyPhotonViewID);
            if (targetRigidbodyPhotonView == null) {
                return;
            }

            var targetRigidbody = targetRigidbodyPhotonView.gameObject.GetComponent<Rigidbody>();
            if (targetRigidbody == null) {
                return;
            }

            targetRigidbody.AddForceAtPosition(force, point, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Sets the rotation of the character.
        /// </summary>
        /// <param name="rotation">The rotation to set.</param>
        /// <param name="snapAnimator">Should the animator be snapped into position?</param>
        public void SetRotation(Quaternion rotation, bool snapAnimator)
        {
            photonView.RPC("SetRotationRPC", RpcTarget.Others, rotation, snapAnimator);
        }

        /// <summary>
        /// Sets the rotation of the character.
        /// </summary>
        /// <param name="rotation">The rotation to set.</param>
        /// <param name="snapAnimator">Should the animator be snapped into position?</param>
        [PunRPC]
        public void SetRotationRPC(Quaternion rotation, bool snapAnimator)
        {
            m_CharacterLocomotion.SetRotation(rotation, snapAnimator);
        }

        /// <summary>
        /// Sets the position of the character.
        /// </summary>
        /// <param name="position">The position to set.</param>
        /// <param name="snapAnimator">Should the animator be snapped into position?</param>
        public void SetPosition(Vector3 position, bool snapAnimator)
        {
            photonView.RPC("SetPositionRPC", RpcTarget.Others, position, snapAnimator);
        }

        /// <summary>
        /// Sets the position of the character.
        /// </summary>
        /// <param name="position">The position to set.</param>
        /// <param name="snapAnimator">Should the animator be snapped into position?</param>
        [PunRPC]
        public void SetPositionRPC(Vector3 position, bool snapAnimator)
        {
            m_CharacterLocomotion.SetPosition(position, snapAnimator);
        }

        /// <summary>
        /// Resets the rotation and position to their default values.
        /// </summary>
        public void ResetRotationPosition()
        {
            // The ViewID may not be initialized yet.
            if (photonView.ViewID == 0) {
                return;
            }

            photonView.RPC("ResetRotationPositionRPC", RpcTarget.Others);
        }

        /// <summary>
        /// Resets the rotation and position to their default values on the network.
        /// </summary>
        [PunRPC]
        public void ResetRotationPositionRPC()
        {
            m_CharacterLocomotion.ResetRotationPosition();
        }

        /// <summary>
        /// Sets the position and rotation of the character on the network.
        /// </summary>
        /// <param name="position">The position to set.</param>
        /// <param name="rotation">The rotation to set.</param>
        /// <param name="snapAnimator">Should the animator be snapped into position?</param>
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation, bool snapAnimator)
        {
            photonView.RPC("SetPositionAndRotationRPC", RpcTarget.Others, position, rotation, snapAnimator);
        }

        /// <summary>
        /// Sets the position and rotation of the character.
        /// </summary>
        /// <param name="position">The position to set.</param>
        /// <param name="rotation">The rotation to set.</param>
        /// <param name="snapAnimator">Should the animator be snapped into position?</param>
        [PunRPC]
        public void SetPositionAndRotationRPC(Vector3 position, Quaternion rotation, bool snapAnimator)
        {
            m_CharacterLocomotion.SetPositionAndRotation(position, rotation, snapAnimator);
        }

        /// <summary>
        /// Activates or deactivates the character.
        /// </summary>
        /// <param name="active">Is the character active?</param>
        /// <param name="uiEvent">Should the OnShowUI event be executed?</param>
        public void SetActive(bool active, bool uiEvent)
        {
            // The ViewID may not be initialized yet.
            if (photonView.ViewID == 0) {
                return;
            }

            photonView.RPC("SetActiveRPC", RpcTarget.Others, active, uiEvent);
        }

        /// <summary>
        /// Activates or deactivates the character on the network.
        /// </summary>
        /// <param name="active">Is the character active?</param>
        /// <param name="uiEvent">Should the OnShowUI event be executed?</param>
        [PunRPC]
        private void SetActiveRPC(bool active, bool uiEvent)
        {
            m_CharacterLocomotion.SetActive(active, uiEvent);
        }

        /// <summary>
        /// The character has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<Ability, bool>(m_GameObject, "OnCharacterAbilityActive", OnAbilityActive);
            EventHandler.UnregisterEvent<ItemAbility, bool>(m_GameObject, "OnCharacterItemAbilityActive", OnItemAbilityActive);
            EventHandler.UnregisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }
    }
}