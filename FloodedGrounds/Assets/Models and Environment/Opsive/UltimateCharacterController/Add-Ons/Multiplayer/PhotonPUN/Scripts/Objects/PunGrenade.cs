/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Objects
{
    /// <summary>
    /// Initializes the grenade over the network.
    /// </summary>
    public class PunGrenade : Grenade, ISpawnDataObject
    {
        private object[] m_SpawnData;

        /// <summary>
        /// Returns the initialization data that is required when the object spawns. This allows the remote players to initialize the object correctly.
        /// </summary>
        /// <returns>The initialization data that is required when the object spawns.</returns>
        public object[] SpawnData()
        {
            if (m_SpawnData == null) {
                m_SpawnData = new object[10];
            }
            m_SpawnData[0] = m_Velocity;
            m_SpawnData[1] = m_Torque;
            m_SpawnData[2] = m_DamageAmount;
            m_SpawnData[3] = m_ImpactForce;
            m_SpawnData[4] = m_ImpactForceFrames;
            m_SpawnData[5] = m_ImpactLayers.value;
            m_SpawnData[6] = m_ImpactStateName;
            m_SpawnData[7] = m_ImpactStateDisableTimer;
            m_SpawnData[8] = m_ScheduledDeactivation != null ? (m_ScheduledDeactivation.EndTime - Time.time) : -1;
            m_SpawnData[9] = m_Originator.GetCachedComponent<PhotonView>().ViewID;

            return m_SpawnData;
        }

        /// <summary>
        /// The object has been spawned. Initialize the grenade.
        /// </summary>
        public void ObjectSpawned()
        {
            var photonView = gameObject.GetCachedComponent<PhotonView>();
            if (photonView == null || photonView.InstantiationData == null) {
                return;
            }

            // Initialize the grenade from the data within the InitializationData field.
            var originator = PhotonNetwork.GetPhotonView((int)photonView.InstantiationData[9]);
            Initialize((Vector3)photonView.InstantiationData[0], (Vector3)photonView.InstantiationData[1], (float)photonView.InstantiationData[2],
                            (float)photonView.InstantiationData[3], (int)photonView.InstantiationData[4], (int)photonView.InstantiationData[5],
                            (string)photonView.InstantiationData[6], (float)photonView.InstantiationData[7], null, originator.gameObject, false);

            // The grenade should start cooking.
            var deactivationTime = (float)photonView.InstantiationData[8];
            if (deactivationTime > 0) {
                m_ScheduledDeactivation = Scheduler.Schedule(deactivationTime, Deactivate);
            }
        }
    }
}
