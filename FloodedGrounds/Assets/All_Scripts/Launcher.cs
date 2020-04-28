/// Created by Emanuel 
/// Handles matchmaking and lobby logic

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace FloodedGrounds
{
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher currentGame;
    
    #region Public Fields
    [Tooltip("The UI panel to let the user enter name, connect, and play")]
    [SerializeField]
    private GameObject controlPanel; 
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    #endregion


    #region Private Serializable Fields

    [Tooltip("Max Number of Players per room; when full, can't be joined by new players")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    [Tooltip("The toggle that switches between a first and third person start.")]
    [SerializeField] protected Toggle m_PerspectiveToggle;

    #endregion

    #region Private Fields

    /// <summary>
    /// Client's version no.!-- Users are separated by gameVersion
    /// </summary>

    //leave this until versions become so different they cannot work together
    string gameVersion = "1";

    bool isConnecting;

    #endregion

    #region MonoBehavior Callbacks

    /// <summary>
    /// MonoBehavior called on gameobject by unity during init phase
    /// </summary>
    void Awake(){
        /// make sure the scene is the same for all players connected
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

    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    /// connects to photon cloud
    public void Connect()
    {

      progressLabel.SetActive(true);
      controlPanel.SetActive(false);  
      if(PhotonNetwork.IsConnected)
      {
        PhotonNetwork.JoinRoom("Test Room");
      }  
      else
      {
          isConnecting = PhotonNetwork.ConnectUsingSettings();
          PhotonNetwork.GameVersion = gameVersion;
      }

    }
    #endregion

    #region MonoBehaviorPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Launcher: ConnectedToMaster called");
        //room specified here: 
        PhotonNetwork.JoinRoom("Test Room");
        isConnecting = false; 
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available, either full or no rooms created");
        RoomOptions roomOptions = new RoomOptions (){ IsVisible = true, MaxPlayers = maxPlayersPerRoom};
        int randomNum = Random.Range(0, 1000000);
        PhotonNetwork.CreateRoom(randomNum.ToString(), roomOptions, TypedLobby.Default);  
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
        Debug.Log("Loading the Level");
        
        // #Critical
        // Load the Room Level.
        PhotonNetwork.LoadLevel("New Game Scene");
        }
    }    


    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("OnDisconnect Called by PUN with reason {0}", cause);
        isConnecting = false; 
    }

    #endregion



}

}