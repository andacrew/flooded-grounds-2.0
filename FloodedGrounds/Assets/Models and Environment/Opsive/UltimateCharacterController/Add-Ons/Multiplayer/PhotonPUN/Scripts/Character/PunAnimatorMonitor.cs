/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Character
{
    /// <summary>
    /// Synchronizes the Ultimate Character Controller animator across the network.
    /// </summary>
    public class PunAnimatorMonitor : AnimatorMonitor, IPunObservable
    {
        private PhotonView m_PhotonView;
        private int m_SnappedAbilityIndex = -1;

        private PhotonStreamQueue m_StreamQueue = new PhotonStreamQueue(120);

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_PhotonView = GetComponent<PhotonView>();
            m_StreamQueue = new PhotonStreamQueue(120);
        }

        /// <summary>
        /// Verify the update mode of the animator.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            // Remote players do not move within the FixedUpdate loop.
            if (!m_PhotonView.IsMine) {
                var animators = GetComponentsInChildren<Animator>(true);
                for (int i = 0; i < animators.Length; ++i) {
                    animators[i].updateMode = AnimatorUpdateMode.Normal;
                }
            }
        }

        /// <summary>
        /// Snaps the animator to the default values.
        /// </summary>
        protected override void SnapAnimator()
        {
            base.SnapAnimator();

            m_SnappedAbilityIndex = AbilityIndex;
        }

        /// <summary>
        /// Reads/writes the continuous animator parameters.
        /// </summary>
        private void Update()
        {
            if (PhotonNetwork.InRoom == false || PhotonNetwork.CurrentRoom.PlayerCount <= 1) {
                m_StreamQueue.Reset();
                return;
            }

            // OnPhotonSerializeView is called within FixedUpdate while certain parameters need to be updated more often for a smooth movement.
            // Update those parameters here.
            if (m_PhotonView.IsMine) {
                m_StreamQueue.SendNext(HorizontalMovement);
                m_StreamQueue.SendNext(ForwardMovement);
                m_StreamQueue.SendNext(Pitch);
                m_StreamQueue.SendNext(Yaw);
                m_StreamQueue.SendNext(AbilityIndex);
                m_StreamQueue.SendNext(AbilityFloatData);
            } else {
                if (m_StreamQueue.HasQueuedObjects()) {
                    SetHorizontalMovementParameter((float)m_StreamQueue.ReceiveNext(), 1);
                    SetForwardMovementParameter((float)m_StreamQueue.ReceiveNext(), 1);
                    SetPitchParameter((float)m_StreamQueue.ReceiveNext(), 1);
                    SetYawParameter((float)m_StreamQueue.ReceiveNext(), 1);
                    var abilityIndex = (int)m_StreamQueue.ReceiveNext();
                    // When the animator is snapped the ability index will be reset. It may take some time for that value to propagate across the network.
                    // Wait to set the ability index until it is the correct reset value.
                    if (m_SnappedAbilityIndex == -1 || abilityIndex == m_SnappedAbilityIndex) {
                        SetAbilityIndexParameter(abilityIndex);
                        m_SnappedAbilityIndex = -1;
                    }
                    SetAbilityFloatDataParameter((float)m_StreamQueue.ReceiveNext(), 1);
                }
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
                stream.SendNext(Speed);
                stream.SendNext(Height);
                stream.SendNext(Moving);
                stream.SendNext(Aiming);
                stream.SendNext(MovementSetID);
                stream.SendNext(AbilityIntData);
                for (int i = 0; i < ParameterSlotCount; ++i) {
                    stream.SendNext(ItemSlotID[i]);
                    stream.SendNext(ItemSlotStateIndex[i]);
                    stream.SendNext(ItemSlotSubstateIndex[i]);
                }
            } else { // Reading.
                m_StreamQueue.Deserialize(stream);
                SetSpeedParameter((float)stream.ReceiveNext(), 1);
                SetHeightParameter((int)stream.ReceiveNext());
                SetMovingParameter((bool)stream.ReceiveNext());
                SetAimingParameter((bool)stream.ReceiveNext());
                SetMovementSetIDParameter((int)stream.ReceiveNext());
                SetAbilityIntDataParameter((int)stream.ReceiveNext());
                for (int i = 0; i < ParameterSlotCount; ++i) {
                    SetItemIDParameter(i, (int)stream.ReceiveNext());
                    SetItemStateIndexParameter(i, (int)stream.ReceiveNext());
                    SetItemSubstateIndexParameter(i, (int)stream.ReceiveNext());
                }
            }
        }

        /// <summary>
        /// Sets the Speed parameter to the specified value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="timeScale">The time scale of the character.</param>
        /// <param name="dampingTime">The time allowed for the parameter to reach the value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetSpeedParameter(float value, float timeScale, float dampingTime)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetSpeedParameter(value, timeScale, dampingTime);
        }

        /// <summary>
        /// Sets the Height parameter to the specified value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetHeightParameter(int value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetHeightParameter(value);
        }

        /// <summary>
        /// Sets the Moving parameter to the specified value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetMovingParameter(bool value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetMovingParameter(value);
        }

        /// <summary>
        /// Sets the Aiming parameter to the specified value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetAimingParameter(bool value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetAimingParameter(value);
        }

        /// <summary>
        /// Sets the Movement Set ID parameter to the specified value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetMovementSetIDParameter(int value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetMovementSetIDParameter(value);
        }

        /// <summary>
        /// Sets the Int Data parameter to the specified value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetAbilityIntDataParameter(int value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetAbilityIntDataParameter(value);
        }

        /// <summary>
        /// Sets the Item ID parameter with the indicated slot to the specified value.
        /// </summary>
        /// <param name="slotID">The slot that the item occupies.</param>
        /// <param name="value">The new value.</param>
        public override bool SetItemIDParameter(int slotID, int value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetItemIDParameter(slotID, value);
        }

        /// <summary>
        /// Sets the Primary Item State Index parameter with the indicated slot to the specified value.
        /// </summary>
        /// <param name="slotID">The slot that the item occupies.</param>
        /// <param name="value">The new value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetItemStateIndexParameter(int slotID, int value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetItemStateIndexParameter(slotID, value);
        }

        /// <summary>
        /// Sets the Item Substate Index parameter with the indicated slot to the specified value.
        /// </summary>
        /// <param name="slotID">The slot that the item occupies.</param>
        /// <param name="value">The new value.</param>
        /// <returns>True if the parameter was changed.</returns>
        public override bool SetItemSubstateIndexParameter(int slotID, int value)
        {
            // The animator may not be enabled. Return silently.
            if (!m_Animator.isActiveAndEnabled) {
                return false;
            }
            return base.SetItemSubstateIndexParameter(slotID, value);
        }
    }
}