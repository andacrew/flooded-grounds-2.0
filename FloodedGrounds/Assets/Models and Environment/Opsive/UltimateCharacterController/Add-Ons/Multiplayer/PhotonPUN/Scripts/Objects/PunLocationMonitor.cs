/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Networking.Game;
using Photon.Pun;
using Photon.Realtime;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Objects
{
    /// <summary>
    /// Synchronizes the object's GameObject, Transform or Rigidbody values over the network.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PunLocationMonitor : MonoBehaviour, IPunObservable
    {
        [Tooltip("Should the GameObject's active state be syncornized?")]
        [SerializeField] protected bool m_SynchronizeActiveState = true;
        [Tooltip("Should the transform's position be synchronized?")]
        [SerializeField] protected bool m_SynchronizePosition = true;
        [Tooltip("Should the transform's rotation be synchronized?")]
        [SerializeField] protected bool m_SynchronizeRotation = true;
        [Tooltip("Should the transform's scale be synchronized?")]
        [SerializeField] protected bool m_SynchronizeScale;

        public bool SynchronizeActiveState { get { return m_SynchronizeActiveState; } set { m_SynchronizeActiveState = value; } }
        public bool SynchronizePosition { get { return m_SynchronizePosition; } set { m_SynchronizePosition = value; } }
        public bool SynchronizeRotation { get { return m_SynchronizeRotation; } set { m_SynchronizeRotation = value; } }
        public bool SynchronizeScale { get { return m_SynchronizeScale; } set { m_SynchronizeScale = value; } }

        private GameObject m_GameObject;
        private Transform m_Transform;
        private Rigidbody m_Rigidbody;
        private PhotonView m_PhotonView;

        private Vector3 m_NetworkPosition;
        private Quaternion m_NetworkRotation;

        private float m_Distance;
        private float m_Angle;
        private bool m_InitialSync = true;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_PhotonView = GetComponent<PhotonView>();

            m_NetworkPosition = m_Transform.position;
            m_NetworkRotation = m_Transform.rotation;
        }

        /// <summary>
        /// The object has been enabled.
        /// </summary>
        private void OnEnable()
        {
            m_InitialSync = true;

            // If the object is pooled then the network object pool will manage the active state.
            if (m_SynchronizeActiveState && NetworkObjectPool.SpawnedWithPool(m_GameObject)) {
                m_SynchronizeActiveState = false;
            }

            if (m_SynchronizeActiveState && m_PhotonView.ViewID != 0 && m_PhotonView.IsMine) {
                m_PhotonView.RPC("SetActiveRPC", RpcTarget.Others, true);
            }
        }

        /// <summary>
        /// Registers for any interested events.
        /// </summary>
        private void Start()
        {
            if (m_PhotonView.IsMine) {
                EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
            }
        }

        /// <summary>
        /// A player has entered the room. Ensure the joining player is in sync with the current game state.
        /// </summary>
        /// <param name="player">The Photon Player that entered the room.</param>
        /// <param name="character">The character that the player controls.</param>
        private void OnPlayerEnteredRoom(Player player, GameObject character)
        {
            if (!m_SynchronizeActiveState || m_PhotonView.ViewID == 0 || NetworkObjectPool.SpawnedWithPool(m_GameObject)) {
                return;
            }

            m_PhotonView.RPC("SetActiveRPC", player, m_GameObject.activeSelf);
        }

        /// <summary>
        /// Activates or deactivates the GameObject on the network.
        /// </summary>
        /// <param name="active">Should the GameObject be activated?</param>
        [PunRPC]
        private void SetActiveRPC(bool active)
        {
            m_GameObject.SetActive(active);
        }

        /// <summary>
        /// Updates the remote object's transform values.
        /// </summary>
        private void Update()
        {
            if (m_PhotonView.IsMine || m_Rigidbody == null) {
                return;
            }

            Synchronize();
        }

        /// <summary>
        /// Updates the remote object's transform values.
        /// </summary>
        private void FixedUpdate()
        {
            if (m_PhotonView.IsMine || m_Rigidbody != null) {
                return;
            }

            Synchronize();
        }

        /// <summary>
        /// Synchronizes the transform.
        /// </summary>
        private void Synchronize()
        {
            // The position and rotation should be applied immediately if it is the first sync.
            if (m_InitialSync) {
                if (m_SynchronizePosition) {
                    m_Transform.position = m_NetworkPosition;
                }
                if (m_SynchronizeRotation) {
                    m_Transform.rotation = m_NetworkRotation;
                }
                m_InitialSync = false;
                return;
            }

            if (m_SynchronizePosition) {
                m_Transform.position = Vector3.MoveTowards(transform.position, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate));
            }
            if (m_SynchronizeRotation) {
                m_Transform.rotation = Quaternion.RotateTowards(transform.rotation, m_NetworkRotation, m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        /// <summary>
        /// Called by PUN several times per second, so that your script can write and read synchronization data for the PhotonView.
        /// </summary>
        /// <param name="stream">The stream that is being written to/read from.</param>
        /// <param name="info">Contains information about the message.</param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) {
                // Send the current GameObject and Transform values to all remote players.
                if (m_SynchronizePosition) {
                    stream.SendNext(m_Transform.position);
                    stream.SendNext(m_Transform.position - m_NetworkPosition);
                    m_NetworkPosition = m_Transform.position;

                    if (m_Rigidbody != null) {
                        stream.SendNext(m_Rigidbody.velocity);
                    }
                }

                if (m_SynchronizeRotation) {
                    stream.SendNext(m_Transform.rotation);

                    if (m_Rigidbody != null) {
                        stream.SendNext(m_Rigidbody.angularVelocity);
                    }
                }

                if (m_SynchronizeScale) {
                    stream.SendNext(m_Transform.localScale);
                }
            } else {
                // Receive the GameObject and Transform values.
                // The position and rotation will then be used within the Update method to actually move the character.
                if (m_SynchronizePosition) {
                    m_NetworkPosition = (Vector3)stream.ReceiveNext();

                    // Compensate for the lag.
                    var lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    m_NetworkPosition += ((Vector3)stream.ReceiveNext()) * lag;
                    m_Distance = Vector3.Distance(m_Transform.position, m_NetworkPosition);

                    if (m_Rigidbody != null) {
                        m_Rigidbody.velocity = (Vector3)stream.ReceiveNext();
                    }
                }

                if (m_SynchronizeRotation) {
                    m_NetworkRotation = (Quaternion)stream.ReceiveNext();
                    m_Angle = Quaternion.Angle(m_Transform.rotation, m_NetworkRotation);

                    if (m_Rigidbody != null) {
                        m_Rigidbody.angularVelocity = (Vector3)stream.ReceiveNext();
                    }
                }

                if (m_SynchronizeScale) {
                    m_Transform.localScale = (Vector3)stream.ReceiveNext();
                }
            }
        }

        /// <summary>
        /// The object has been deactivated.
        /// </summary>
        private void OnDisable()
        {
            if (m_SynchronizeActiveState && m_PhotonView.ViewID != 0 && PhotonNetwork.IsConnected) {
                m_PhotonView.RPC("SetActiveRPC", RpcTarget.Others, false);
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