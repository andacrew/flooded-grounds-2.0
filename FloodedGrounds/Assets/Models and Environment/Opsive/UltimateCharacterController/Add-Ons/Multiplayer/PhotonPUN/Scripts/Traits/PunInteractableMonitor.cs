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
    /// Synchronizes the Interactable component over the network.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PunInteractableMonitor : MonoBehaviour, INetworkInteractableMonitor
    {
        private GameObject m_GameObject;
        private Interactable m_Interactable;
        private PhotonView m_PhotonView;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            m_GameObject = gameObject;
            m_Interactable = m_GameObject.GetCachedComponent<Interactable>();
            m_PhotonView = m_GameObject.GetCachedComponent<PhotonView>();
        }

        /// <summary>
        /// Performs the interaction.
        /// </summary>
        /// <param name="character">The character that wants to interactact with the target.</param>
        public void Interact(GameObject character)
        {
            var characterPhotonView = character.GetCachedComponent<PhotonView>();
            if (characterPhotonView == null) {
                Debug.LogError("Error: The character " + character.name + " must have a PhotonView component.");
                return;
            }

            m_PhotonView.RPC("InteractRPC", RpcTarget.Others, characterPhotonView.ViewID);
        }

        /// <summary>
        /// Performs the interaction on the network.
        /// </summary>
        /// <param name="characterViewID">The View ID of the character that performed the interaction.</param>
        [PunRPC]
        private void InteractRPC(int characterViewID)
        {
            var characterPhotonView = PhotonNetwork.GetPhotonView(characterViewID);
            if (characterPhotonView == null) {
                return;
            }

            m_Interactable.Interact(characterPhotonView.gameObject);
        }
    }
}