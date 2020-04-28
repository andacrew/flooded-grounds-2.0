/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Demo
{
    /// <summary>
    /// Simple menu for connecting to the Photon game scene.
    /// </summary>
    public class MenuManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("Should the game automatically connect to a room?")]
        [SerializeField] protected bool m_AutoConnect;
        [Tooltip("The maximum number of players that can join a single room.")]
        [SerializeField] protected int m_MaxPlayerCount = 8;
        [Tooltip("The button that starts connecting to a room.")]
        [SerializeField] protected Button m_ConnectButton;
        [Tooltip("UI showing the status of the connection.")]
        [SerializeField] protected Text m_Status;
        [Tooltip("The name of the scene to load when the player has joined a room.")]
        [SerializeField] protected string m_SceneName = "DemoRoom";
        [Tooltip("The toggle that switches between a first and third person start.")]
        [SerializeField] protected Toggle m_PerspectiveToggle;

        private bool m_IsConnecting;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            if (m_PerspectiveToggle != null) {
#if !FIRST_PERSON_CONTROLLER
                m_PerspectiveToggle.isOn = false;
                m_PerspectiveToggle.interactable = false;
#elif !THIRD_PERSON_CONTROLLER
                m_PerspectiveToggle.isOn = true;
                m_PerspectiveToggle.interactable = false;
#else
                if (PlayerPrefs.HasKey("START_PERSPECTIVE")) {
                    m_PerspectiveToggle.isOn = PlayerPrefs.GetInt("START_PERSPECTIVE", m_PerspectiveToggle.isOn ? 1 : 0) == 1;
                } else {
                    PlayerPrefs.SetInt("START_PERSPECTIVE", m_PerspectiveToggle.isOn ? 1 : 0);
                }
#endif
            }
        }

#if FIRST_PERSON_CONTROLLER && THIRD_PERSON_CONTROLLER
        /// <summary>
        /// The start perspective has been toggled.
        /// </summary>
        public void PerspectiveToggled()
        {
            PlayerPrefs.SetInt("START_PERSPECTIVE", m_PerspectiveToggle.isOn ? 1 : 0);
        }
#endif

        /// <summary>
        /// Auto connect to a room.
        /// </summary>
        private void Start()
        {
            if (m_AutoConnect || m_ConnectButton == null) {
                Connect();
            }
        }

        /// <summary>
        /// Connects to the Photon Network if not otherwise connected, otherwise join a random room.
        /// </summary>
        public void Connect()
        {
            if (m_IsConnecting) {
                return;
            }

            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.JoinRandomRoom();
                SetStatus("Joining an existing room.");
            } else {
                PhotonNetwork.GameVersion = UltimateCharacterController.Utility.AssetInfo.Version;
                PhotonNetwork.ConnectUsingSettings();
                SetStatus("Connecting to Photon Network");
            }

            m_IsConnecting = true;
            if (m_ConnectButton != null) {
                m_ConnectButton.interactable = false;
            }
            if (m_PerspectiveToggle != null) {
                m_PerspectiveToggle.interactable = false;
            }
        }

        /// <summary>
        /// The player has connected to the Photon Network.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            if (m_IsConnecting) {
                PhotonNetwork.JoinRandomRoom();
                SetStatus("Joining an existing room.");
            }
        }

        /// <summary>
        /// There are no rooms available. Create a new room.
        /// </summary>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)m_MaxPlayerCount;
            PhotonNetwork.CreateRoom(null, roomOptions);
            SetStatus("No rooms available. Creating a room.");
        }

        /// <summary>
        /// The player has joined a room. Load the game scene.
        /// </summary>
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
                PhotonNetwork.LoadLevel(m_SceneName);
            }
            SetStatus("Connected to a room. Loading the level.");
        }

        /// <summary>
        /// Sets the status text to the specified value.
        /// </summary>
        /// <param name="status">The status text value.</param>
        private void SetStatus(string status)
        {
            if (m_Status == null) {
                return;
            }

            m_Status.text = status;
        }
    }
}