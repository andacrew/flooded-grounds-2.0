/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Networking;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun
{
    /// <summary>
    /// Contains information about the object on the network.
    /// </summary>
    public class PunNetworkInfo : MonoBehaviour, INetworkInfo
    {
        private PhotonView m_PhotonView;

        private void Awake()
        {
            m_PhotonView = gameObject.GetCachedComponent<PhotonView>();
        }

        /// <summary>
        /// Is the networking implementation server authoritative?
        /// </summary>
        /// <returns>True if the network transform is server authoritative.</returns>
        public bool IsServerAuthoritative()
        {
            return false;
        }

        /// <summary>
        /// Is the game instance on the server?
        /// </summary>
        /// <returns>True if the game instance is on the server.</returns>
        public bool IsServer()
        {
            return PhotonNetwork.IsMasterClient;
        }

        /// <summary>
        /// Is the character the local player?
        /// </summary>
        /// <returns>True if the character is the local player.</returns>
        public bool IsLocalPlayer()
        {
            return m_PhotonView.IsMine;
        }
    }
}