/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.Objects.CharacterAssist;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Objects
{
    /// <summary>
    /// Initializes the item pickup over the network.
    /// </summary>
    public class PunItemPickup : ItemPickup, ISpawnDataObject
    {
        private object[] m_SpawnData;

        private TrajectoryObject m_TrajectoryObject;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_TrajectoryObject = gameObject.GetCachedComponent<TrajectoryObject>();
        }

        /// <summary>
        /// Returns the initialization data that is required when the object spawns. This allows the remote players to initialize the object correctly.
        /// </summary>
        /// <returns>The initialization data that is required when the object spawns.</returns>
        public object[] SpawnData()
        {
            var objLength = m_ItemTypeCounts.Length * 2 + (m_TrajectoryObject != null ? 3 : 0);
            if (m_SpawnData == null) {
                m_SpawnData = new object[objLength];
            } else if (m_SpawnData.Length != objLength) {
                System.Array.Resize(ref m_SpawnData, objLength);
            }

            for (int i = 0; i < m_ItemTypeCounts.Length; ++i) {
                m_SpawnData[i * 2] = m_ItemTypeCounts[i].ItemType.ID;
                m_SpawnData[i * 2 + 1] = m_ItemTypeCounts[i].Count;
            }

            // The trajectory data needs to also be sent.
            if (m_TrajectoryObject != null) {
                m_SpawnData[m_SpawnData.Length - 3] = m_TrajectoryObject.Velocity;
                m_SpawnData[m_SpawnData.Length - 2] = m_TrajectoryObject.Torque;
                var originatorID = -1;
                if (m_TrajectoryObject.Originator != null) {
                    var originatorView = m_TrajectoryObject.Originator.GetCachedComponent<PhotonView>();
                    if (originatorView != null) {
                        originatorID = originatorView.ViewID;
                    }
                }
                m_SpawnData[m_SpawnData.Length - 1] = originatorID;
            }

            return m_SpawnData;
        }

        /// <summary>
        /// The object has been spawned. Initialize the item pickup.
        /// </summary>
        public void ObjectSpawned()
        {
            var photonView = gameObject.GetCachedComponent<PhotonView>();
            if (photonView == null || photonView.InstantiationData == null) {
                return;
            }

            // Return the old.
            for (int i = 0; i < m_ItemTypeCounts.Length; ++i) {
                ObjectPool.Return(m_ItemTypeCounts[i]);
            }

            // Setup the item counts.
            var itemTypeCountLength = (photonView.InstantiationData.Length - (m_TrajectoryObject != null ? 3 : 0)) / 2;
            if (m_ItemTypeCounts.Length != itemTypeCountLength) {
                m_ItemTypeCounts = new ItemTypeCount[itemTypeCountLength];
            }
            for (int i = 0; i < itemTypeCountLength; ++i) {
                var itemTypeCount = ObjectPool.Get<ItemTypeCount>();
                itemTypeCount.Initialize(ItemTypeTracker.GetItemType((int)photonView.InstantiationData[i * 2]), (float)photonView.InstantiationData[i * 2 + 1]);
                m_ItemTypeCounts[i] = itemTypeCount;
            }
            Initialize(true);

            // Setup the trajectory object.
            if (m_TrajectoryObject != null) {
                var velocity = (Vector3)photonView.InstantiationData[photonView.InstantiationData.Length - 3];
                var torque = (Vector3)photonView.InstantiationData[photonView.InstantiationData.Length - 2];
                var originatorID = (int)photonView.InstantiationData[photonView.InstantiationData.Length - 1];
                GameObject originator = null;
                if (originatorID != -1) {
                    var originatorView = PhotonNetwork.GetPhotonView(originatorID);
                    if (originatorView != null) {
                        originator = originatorView.gameObject;
                    }
                }
                m_TrajectoryObject.Initialize(velocity, torque, originator);
            }
        }
    }
}