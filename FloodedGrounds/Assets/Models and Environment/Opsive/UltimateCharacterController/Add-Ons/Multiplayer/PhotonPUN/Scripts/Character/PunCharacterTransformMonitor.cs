/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Character
{
    /// <summary>
    /// Synchronizes the character's transform values over the network.
    /// </summary>
    public class PunCharacterTransformMonitor : MonoBehaviour, IPunObservable
    {
        [Tooltip("Should the transform's scale be synchronized?")]
        [SerializeField] protected bool m_SynchronizeScale;
        [Tooltip("A multiplier to apply to the interpolation destination for remote players.")]
        [SerializeField] protected float m_RemoteInterpolationMultiplayer = 1.2f;

        private Transform m_Transform;
        private PhotonView m_PhotonView;
        private UltimateCharacterLocomotion m_CharacterLocomotion;

        private Vector3 m_NetworkPosition;
        private Quaternion m_NetworkRotation;

        private int m_PlatformPhotonViewID;
        private Transform m_NetworkPlatform;
        private Quaternion m_NetworkPlatformRotationOffset;
        private Quaternion m_NetworkPlatformPrevRotationOffset;
        private Vector3 m_NetworkPlatformRelativePosition;
        private Vector3 m_NetworkPlatformPrevRelativePosition;

        private float m_Distance;
        private float m_Angle;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_Transform = transform;
            m_PhotonView = gameObject.GetCachedComponent<PhotonView>();
            m_CharacterLocomotion = gameObject.GetCachedComponent<UltimateCharacterLocomotion>();

            m_NetworkPosition = m_Transform.position;
            m_NetworkRotation = m_Transform.rotation;

            EventHandler.RegisterEvent(gameObject, "OnRespawn", OnRespawn);
            EventHandler.RegisterEvent<bool>(gameObject, "OnCharacterImmediateTransformChange", OnImmediateTransformChange);
        }

        /// <summary>
        /// Updates the remote character's transform values.
        /// </summary>
        private void Update()
        {
            // Local players will move using the regular UltimateCharacterLocomotion.Move method.
            if (m_PhotonView.IsMine) {
                return;
            }

            // When the character is on a moving platform the position and rotation is relative to that platform. This allows the character to stay on the platform
            // even though the platform will not be in the exact same location between any two instances.
            var serializationRate = (1f / PhotonNetwork.SerializationRate) * m_RemoteInterpolationMultiplayer;
            if (m_NetworkPlatform != null) {
                m_NetworkPlatformPrevRelativePosition = Vector3.MoveTowards(m_NetworkPlatformPrevRelativePosition, m_NetworkPlatformRelativePosition, m_Distance * serializationRate);
                m_CharacterLocomotion.SetPosition(m_NetworkPlatform.TransformPoint(m_NetworkPlatformPrevRelativePosition), false);

                m_NetworkPlatformPrevRotationOffset = Quaternion.RotateTowards(m_NetworkPlatformPrevRotationOffset, m_NetworkPlatformRotationOffset, m_Angle * serializationRate);
                m_CharacterLocomotion.SetRotation(MathUtility.TransformQuaternion(m_NetworkPlatform.rotation, m_NetworkPlatformPrevRotationOffset), false);
            } else {
                m_Transform.position = Vector3.MoveTowards(m_Transform.position, m_NetworkPosition, m_Distance * serializationRate);
                m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, m_NetworkRotation, m_Angle * serializationRate);
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
                // When the character is on a platform the position and rotation is relative to that platform.
                if (m_CharacterLocomotion.Platform != null) {
                    var platformPhotonView = m_CharacterLocomotion.Platform.gameObject.GetCachedComponent<PhotonView>();
                    if (platformPhotonView == null) {
                        Debug.LogError("Error: The platform " + m_CharacterLocomotion.Platform + " must have a PhotonView.");
                        return;
                    }

                    stream.SendNext(platformPhotonView.ViewID);
                    stream.SendNext(m_CharacterLocomotion.Platform.InverseTransformPoint(m_Transform.position));
                    stream.SendNext(MathUtility.InverseTransformQuaternion(m_CharacterLocomotion.Platform.rotation, m_Transform.rotation));
                } else {
                    stream.SendNext(-1);
                    stream.SendNext(m_Transform.position);
                    stream.SendNext(m_Transform.position - m_NetworkPosition);
                    stream.SendNext(m_Transform.rotation);
                }
                m_NetworkPosition = m_Transform.position;

                if (m_SynchronizeScale) {
                    stream.SendNext(m_Transform.localScale);
                }

            } else {
                // When the character is on a platform the position and rotation is relative to that platform.
                var platformPhotonViewID = (int)stream.ReceiveNext();
                if (platformPhotonViewID != -1) {
                    m_NetworkPlatformRelativePosition = (Vector3)stream.ReceiveNext();
                    m_NetworkPlatformRotationOffset = (Quaternion)stream.ReceiveNext();

                    // Do not do any sort of interpolation when the platform has changed.
                    if (platformPhotonViewID != m_PlatformPhotonViewID) {
                        m_NetworkPlatform = PhotonNetwork.GetPhotonView(platformPhotonViewID).transform;
                        m_NetworkPlatformRelativePosition = m_NetworkPlatformPrevRelativePosition = m_NetworkPlatform.InverseTransformPoint(m_Transform.position);
                        m_NetworkPlatformRotationOffset = m_NetworkPlatformPrevRotationOffset = MathUtility.InverseTransformQuaternion(m_NetworkPlatform.rotation, m_Transform.rotation);
                    }

                    m_Distance = Vector3.Distance(m_NetworkPlatformPrevRelativePosition, m_NetworkPlatformRelativePosition);
                    m_Angle = Quaternion.Angle(m_NetworkPlatformPrevRotationOffset, m_NetworkPlatformRotationOffset);
                } else {
                    if (m_PlatformPhotonViewID != -1) {
                        m_NetworkPlatform = null;
                    }
                    m_NetworkPosition = (Vector3)stream.ReceiveNext();
                    // Account for the lag.
                    var lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    m_NetworkPosition += ((Vector3)stream.ReceiveNext()) * lag;
                    m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    m_Distance = Vector3.Distance(m_Transform.position, m_NetworkPosition);
                    m_Angle = Quaternion.Angle(m_Transform.rotation, m_NetworkRotation);
                }
                m_PlatformPhotonViewID = platformPhotonViewID;

                if (m_SynchronizeScale) {
                    m_Transform.localScale = (Vector3)stream.ReceiveNext();
                }
            }
        }

        /// <summary>
        /// The character has respawned.
        /// </summary>
        private void OnRespawn()
        {
            m_NetworkPosition = m_Transform.position;
            m_NetworkRotation = m_Transform.rotation;
        }

        /// <summary>
        /// The character's position or rotation has been teleported.
        /// </summary>
        /// <param name="snapAnimator">Should the animator be snapped?</param>
        private void OnImmediateTransformChange(bool snapAnimator)
        {
            m_NetworkPosition = m_Transform.position;
            m_NetworkRotation = m_Transform.rotation;
        }

        /// <summary>
        /// The character has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent(gameObject, "OnRespawn", OnRespawn);
            EventHandler.UnregisterEvent<bool>(gameObject, "OnCharacterImmediateTransformChange", OnImmediateTransformChange);
        }
    }
}