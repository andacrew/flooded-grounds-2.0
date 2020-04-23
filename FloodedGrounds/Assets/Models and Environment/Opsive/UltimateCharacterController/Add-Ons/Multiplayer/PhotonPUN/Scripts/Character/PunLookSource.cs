/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Photon.Pun;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Utility;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Character
{
    /// <summary>
    /// Syncronizes the ILookSource over the network.
    /// </summary>
    public class PunLookSource : MonoBehaviour, IPunObservable, ILookSource
    {
        private GameObject m_GameObject;
        private Transform m_Transform;
        private UltimateCharacterLocomotion m_CharacterLocomotion;
        private PhotonView m_PhotonView;
        private ILookSource m_LookSource;

        public GameObject GameObject { get { return m_GameObject; } }
        public Transform Transform { get { return m_Transform; } }
        public float LookDirectionDistance { get { return m_LookDirectionDistance; } }
        public float Pitch { get { return m_Pitch; } }

        private PhotonStreamQueue m_StreamQueue = new PhotonStreamQueue(120);
        private float m_LookDirectionDistance = 1;
        private float m_Pitch;
        private Vector3 m_LookPosition;
        private Vector3 m_LookDirection;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_CharacterLocomotion = m_GameObject.GetCachedComponent<UltimateCharacterLocomotion>();
            m_PhotonView = m_GameObject.GetCachedComponent<PhotonView>();

            m_LookPosition = m_Transform.position;
            m_LookDirection = m_Transform.forward;

            EventHandler.RegisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
        }

        /// <summary>
        /// Register for any interested events.
        /// </summary>
        private void Start()
        {
            // Remote characters will not have a local look source. The current component should act as the look source.
            if (!m_PhotonView.IsMine) {
                EventHandler.UnregisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
                EventHandler.ExecuteEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", this);
            }
        }

        /// <summary>
        /// A new ILookSource object has been attached to the character.
        /// </summary>
        /// <param name="lookSource">The ILookSource object attached to the character.</param>
        private void OnAttachLookSource(ILookSource lookSource)
        {
            m_LookSource = lookSource;
        }

        /// <summary>
        /// Returns the position of the look source.
        /// </summary>
        /// <returns>The position of the look source.</returns>
        public Vector3 LookPosition()
        {
            return m_LookPosition;
        }

        /// <summary>
        /// Returns the direction that the character is looking.
        /// </summary>
        /// <param name="characterLookDirection">Is the character look direction being retrieved?</param>
        /// <returns>The direction that the character is looking.</returns>
        public Vector3 LookDirection(bool characterLookDirection)
        {
            if (characterLookDirection) {
                return m_Transform.forward;
            }
            return m_LookDirection;
        }

        /// <summary>
        /// Returns the direction that the character is looking.
        /// </summary>
        /// <param name="lookPosition">The position that the character is looking from.</param>
        /// <param name="characterLookDirection">Is the character look direction being retrieved?</param>
        /// <param name="layerMask">The LayerMask value of the objects that the look direction can hit.</param>
        /// <param name="useRecoil">Should recoil be included in the look direction?</param>
        /// <returns>The direction that the character is looking.</returns>
        public Vector3 LookDirection(Vector3 lookPosition, bool characterLookDirection, int layerMask, bool useRecoil)
        {
            var collisionLayerEnabled = m_CharacterLocomotion.CollisionLayerEnabled;
            m_CharacterLocomotion.EnableColliderCollisionLayer(false);

            // Cast a ray from the look source point in the forward direction. The look direction is then the vector from the look position to the hit point.
            RaycastHit hit;
            Vector3 direction;
            if (Physics.Raycast(m_LookPosition, m_LookDirection, out hit, m_LookDirectionDistance, layerMask, QueryTriggerInteraction.Ignore)) {
                direction = (hit.point - lookPosition).normalized;
            } else {
                direction = m_LookDirection;
            }

            m_CharacterLocomotion.EnableColliderCollisionLayer(collisionLayerEnabled);
            return direction;
        }

        /// <summary>
        /// Updates the LookSource values.
        /// </summary>
        public void Update()
        {
            if (PhotonNetwork.InRoom == false || PhotonNetwork.CurrentRoom.PlayerCount <= 1) {
                m_StreamQueue.Reset();
                return;
            }

            // The look source variables are continuous and should be updated every frame.
            if (m_PhotonView.IsMine) {
                m_StreamQueue.SendNext(m_LookSource.LookDirectionDistance);
                m_StreamQueue.SendNext(m_LookSource.Pitch);
                m_StreamQueue.SendNext(m_LookSource.LookPosition());
                m_StreamQueue.SendNext(m_LookSource.LookDirection(false));
            } else if (m_StreamQueue.HasQueuedObjects()) {
                m_LookDirectionDistance = (float)m_StreamQueue.ReceiveNext();
                m_Pitch = (float)m_StreamQueue.ReceiveNext();
                m_LookPosition = (Vector3)m_StreamQueue.ReceiveNext();
                m_LookDirection = (Vector3)m_StreamQueue.ReceiveNext();
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
                m_StreamQueue.Serialize(stream);
            } else {
                m_StreamQueue.Deserialize(stream);
            }
        }

        /// <summary>
        /// The character has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
        }
    }
}