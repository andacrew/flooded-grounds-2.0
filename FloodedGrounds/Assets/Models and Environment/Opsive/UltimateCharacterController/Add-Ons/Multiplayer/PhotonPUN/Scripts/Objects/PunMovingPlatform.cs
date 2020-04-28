/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.StateSystem;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Objects
{
    /// <summary>
    /// Syncronizes the moving platform when a new player joins the room.
    /// </summary>
    public class PunMovingPlatform : MovingPlatform, IOnEventCallback
    {
        private PhotonView m_PhotonView;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_PhotonView = gameObject.GetCachedComponent<PhotonView>();
        }

        /// <summary>
        /// The object has been enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            PhotonNetwork.AddCallbackTarget(this);
        }

        /// <summary>
        /// A event from Photon has been sent.
        /// </summary>
        /// <param name="photonEvent">The Photon event.</param>
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == PhotonEventIDs.RemotePlayerInstantiationComplete) {
                if (!PhotonNetwork.IsMasterClient) {
                    return;
                }

                var nextWaypointEventDelay = -1f;
                if (m_NextWaypointEvent != null) {
                    nextWaypointEventDelay = m_NextWaypointEvent.EndTime - Time.time;
                }
                var activeStates = 0;
                for (int i = 0; i < States.Length - 1; ++i) {
                    if (States[i].Active) {
                        activeStates |= (int)Mathf.Pow(i + 1, 2);
                    }
                }
                var player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
                m_PhotonView.RPC("InitializeMovingPlatformRPC", player, m_Transform.position, m_Transform.rotation, activeStates, (int)m_Direction, m_NextWaypoint, m_PreviousWaypoint, m_NextWaypointDistance,
                                                                        nextWaypointEventDelay, m_OriginalRotation, m_MoveTime, m_TargetPosition, m_TargetRotation, m_ActiveCharacterCount);
            }
        }

        /// <summary>
        /// Initialize the moving platform to the same parameters as the master client.
        /// </summary>
        [PunRPC]
        private void InitializeMovingPlatformRPC(Vector3 position, Quaternion rotation, int activeStates, int pathDirection, int nextWaypoint, int previousWaypoint, float nextWaypointDistance,
                                                    float nextWaypointEventDelay, Quaternion originalRotation, float moveTime, Vector3 targetPosition, Quaternion targetRotation,
                                                    int activeCharacterCount, PhotonMessageInfo info)
        {
            m_Transform.position = position;
            m_Transform.rotation = rotation;
            KinematicObjectManager.SetKinematicObjectPosition(KinematicObjectIndex, position);
            KinematicObjectManager.SetKinematicObjectRotation(KinematicObjectIndex, rotation);
            m_Direction = (PathDirection)pathDirection;
            m_NextWaypoint = nextWaypoint;
            m_PreviousWaypoint = previousWaypoint;
            m_NextWaypointDistance = nextWaypointDistance;
            m_OriginalRotation = originalRotation;
            m_MoveTime = moveTime;
            m_TargetPosition = targetPosition;
            m_TargetRotation = targetRotation;
            m_ActiveCharacterCount = activeCharacterCount;
            if (m_NextWaypointEvent != null) {
                Scheduler.Cancel(m_NextWaypointEvent);
                m_NextWaypointEvent = null;
            }
            if (nextWaypointEventDelay != -1) {
                m_NextWaypointEvent = Scheduler.ScheduleFixed(nextWaypointEventDelay, UpdateWaypoint);
            }
            // The states should match the master client.
            for (int i = 0; i < States.Length - 1; ++i) {
                if (((int)Mathf.Pow(i + 1, 2) & activeStates) != 0) {
                    StateManager.SetState(m_GameObject, States[i].Name, true);
                }
            }

            // There will be a small amount of lag between the time that the RPC was sent on the server and the time that it was received on the client.
            // Make up for this difference by simulating the movement for the lag difference.
            var lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            var startTime = Time.time;
            var elapsedTime = 0f;
            while (elapsedTime < lag) {
                // The next waypoint event has to be simulated.
                if (m_NextWaypointEvent != null) {
                    if (startTime + elapsedTime > m_NextWaypointEvent.EndTime) {
                        UpdateWaypoint();
                    }
                }
                Move();
                elapsedTime += Time.fixedDeltaTime;
            }
            KinematicObjectManager.SetKinematicObjectPosition(KinematicObjectIndex, m_Transform.position);
            KinematicObjectManager.SetKinematicObjectRotation(KinematicObjectIndex, m_Transform.rotation);
        }

        /// <summary>
        /// The object has been disabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}