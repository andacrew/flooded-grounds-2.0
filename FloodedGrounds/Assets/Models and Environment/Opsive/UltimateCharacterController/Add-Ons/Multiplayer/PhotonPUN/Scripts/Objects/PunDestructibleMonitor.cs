using UnityEngine;
using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.Networking.Objects;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Objects
{
    /// <summary>
    /// Destroys a Destructible over the network.
    /// </summary>
    [RequireComponent(typeof(PunNetworkInfo))]
    public class PunDestructibleMonitor : MonoBehaviour, IDestructibleMonitor
    {
        private PhotonView m_PhotonView;
        private Destructible m_Destructible;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();
            m_Destructible = GetComponent<Destructible>();
        }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        /// <param name="hitPosition">The position of the destruction.</param>
        /// <param name="hitNormal">The normal direction of the destruction.</param>
        public void Destruct(Vector3 hitPosition, Vector3 hitNormal)
        {
            m_PhotonView.RPC("DestructRPC", RpcTarget.Others, hitPosition, hitNormal);
        }

        /// <summary>
        /// Destroys the object over the network.
        /// </summary>
        /// <param name="hitPosition">The position of the destruction.</param>
        /// <param name="hitNormal">The normal direction of the destruction.</param>
        [PunRPC]
        private void DestructRPC(Vector3 hitPosition, Vector3 hitNormal)
        {
            m_Destructible.Destruct(hitPosition, hitNormal);
        }
    }
}