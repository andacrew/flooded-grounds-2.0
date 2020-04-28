/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Networking.Traits;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Traits
{
    /// <summary>
    /// Synchronizes the Respawner over the network.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PunRespawnerMonitor : MonoBehaviour, INetworkRespawnerMonitor
    {
        private Respawner m_Respawner;
        private PhotonView m_PhotonView;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            m_Respawner = gameObject.GetCachedComponent<Respawner>();
            m_PhotonView = gameObject.GetCachedComponent<PhotonView>();
        }

        /// <summary>
        /// Does the respawn by setting the position and rotation to the specified values.
        /// Enable the GameObject and let all of the listening objects know that the object has been respawned.
        /// </summary>
        /// <param name="position">The respawn position.</param>
        /// <param name="rotation">The respawn rotation.</param>
        /// <param name="transformChange">Was the position or rotation changed?</param>
        public void Respawn(Vector3 position, Quaternion rotation, bool transformChange)
        {
            m_PhotonView.RPC("RespawnRPC", RpcTarget.Others, position, rotation, transformChange);
        }

        /// <summary>
        /// Does the respawn on the network by setting the position and rotation to the specified values.
        /// Enable the GameObject and let all of the listening objects know that the object has been respawned.
        /// </summary>
        /// <param name="position">The respawn position.</param>
        /// <param name="rotation">The respawn rotation.</param>
        /// <param name="transformChange">Was the position or rotation changed?</param>
        [PunRPC]
        private void RespawnRPC(Vector3 position, Quaternion rotation, bool transformChange)
        {
            m_Respawner.Respawn(position, rotation, transformChange);
        }
    }
}