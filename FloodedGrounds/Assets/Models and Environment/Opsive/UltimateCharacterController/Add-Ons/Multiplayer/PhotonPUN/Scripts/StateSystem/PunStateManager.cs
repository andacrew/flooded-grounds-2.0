/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.StateSystem;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.StateSystem
{
    /// <summary>
    /// Ensures the states are synchronized when a new player joins the room. StateManager.SendStateChangeEvent must be enabled for this component to work.
    /// </summary>
    public class PunStateManager : MonoBehaviour, IOnEventCallback
    {
        private Dictionary<GameObject, HashSet<string>> m_ActiveCharacterStates = new Dictionary<GameObject, HashSet<string>>();

        private RaiseEventOptions m_RaisedEventOptions;
        private SendOptions m_ReliableSendOptions;
        private int[] m_TargetActors;
        private object[] m_EventData = new object[3];

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            m_RaisedEventOptions = new RaiseEventOptions {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.DoNotCache
            };

            m_ReliableSendOptions = new SendOptions {
                Reliability = true
            };

            EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
            EventHandler.RegisterEvent<Player, GameObject>("OnPlayerLeftRoom", OnPlayerLeftRoom);
            EventHandler.RegisterEvent<GameObject, string, bool>("OnStateChange", OnStateChange);
        }

        /// <summary>
        /// The object has been enabled.
        /// </summary>
        public void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        /// <summary>
        /// Ensure StateManager.SendStateChangeEvent is true.
        /// </summary>
        private void Start()
        {
            var stateManager = GameObject.FindObjectOfType<StateManager>();
            stateManager.SendStateChangeEvent = true;
        }

        /// <summary>
        /// A player has entered the room. Ensure the joining player is in sync with the current game state.
        /// </summary>
        /// <param name="player">The Photon Player that entered the room.</param>
        /// <param name="character">The character that the player controls.</param>
        private void OnPlayerEnteredRoom(Player player, GameObject character)
        {
            var playerPhotonView = character.GetCachedComponent<PhotonView>();
            if (m_TargetActors == null) {
                m_TargetActors = new int[1];
            }
            m_TargetActors[0] = playerPhotonView.OwnerActorNr;
            m_RaisedEventOptions.TargetActors = m_TargetActors;

            // Ensure the new player has received all of the active events.
            m_EventData[2] = true;
            foreach (var activeStates in m_ActiveCharacterStates) {
                var existingPhotonView = activeStates.Key.GetCachedComponent<PhotonView>();
                foreach (var activestate in activeStates.Value) {
                    m_EventData[0] = existingPhotonView.ViewID;
                    m_EventData[1] = activestate;
                    PhotonNetwork.RaiseEvent(PhotonEventIDs.StateChange, m_EventData, m_RaisedEventOptions, m_ReliableSendOptions);
                }
            }

            // Keep track of the character states for as long as the character is within the room.
            m_ActiveCharacterStates.Add(character, new HashSet<string>());
        }

        /// <summary>
        /// A player has left the room. Perform any cleanup.
        /// </summary>
        /// <param name="player">The Photon Player that left the room.</param>
        /// <param name="character">The character that the player controlled.</param>
        private void OnPlayerLeftRoom(Player player, GameObject character)
        {
            m_ActiveCharacterStates.Remove(character);
        }

        /// <summary>
        /// A state has changed. 
        /// </summary>
        /// <param name="character">The character that had the state change.</param>
        /// <param name="stateName">The name of the state that was changed.</param>
        /// <param name="active">Is the state active?</param>
        private void OnStateChange(GameObject character, string stateName, bool active)
        {
            HashSet<string> activeStates;
            if (!m_ActiveCharacterStates.TryGetValue(character, out activeStates)) {
                return;
            }

            // Store the active states in a HashSet. This will be stored for all characters.
            if (active) {
                activeStates.Add(stateName);
            } else {
                activeStates.Remove(stateName);
            }

            var photonView = character.GetCachedComponent<PhotonView>();
            if (!photonView.IsMine) {
                return;
            }

            // Notify remote players of the state change for the local character.
            m_RaisedEventOptions.TargetActors = null;
            m_EventData[0] = photonView.ViewID;
            m_EventData[1] = stateName;
            m_EventData[2] = active;
            PhotonNetwork.RaiseEvent(PhotonEventIDs.StateChange, m_EventData, m_RaisedEventOptions, m_ReliableSendOptions);
        }

        /// <summary>
        /// A event from Photon has been sent.
        /// </summary>
        /// <param name="photonEvent">The Photon event.</param>
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == PhotonEventIDs.StateChange) {
                var data = (object[])photonEvent.CustomData;
                var photonView = PhotonNetwork.GetPhotonView((int)data[0]);
                if (photonView != null) {
                    StateManager.SetState(photonView.gameObject, (string)data[1], (bool)data[2]);
                }
            }
        }

        /// <summary>
        /// The object has been disabled.
        /// </summary>
        public void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        /// <summary>
        /// The object has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
            EventHandler.UnregisterEvent<Player, GameObject>("OnPlayerLeftRoom", OnPlayerLeftRoom);
            EventHandler.UnregisterEvent<GameObject, string, bool>("OnStateChange", OnStateChange);
        }
    }
}