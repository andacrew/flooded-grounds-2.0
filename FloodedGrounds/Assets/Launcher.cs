/// Created by Emanuel 
/// Handles matchmaking and lobby logic

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
namespace FloodedGrounds
{
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher currentGame;
    #region Private Serializable Fields
    [Tooltip("Max Number of Players per room; when full, can't be joined by new players")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    #endregion

    #region Private Fields


    /// <summmary>
    /// Client's version no.!-- Users are separated by gameVersion
    /// </summary>

    //leave this until versions become so different they cannot work together
    string gameVersion = "1";

    #endregion

    #region MonoBehavior Callbacks

    /// <summary>
    /// MonoBehavior called on gameobject by unity during init phase
    /// </summary>
    void Awake(){
        /// make sure the scene is the same for all players connected
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Connect();
    }

    /// connects to photon cloud
    void Connect()
    {
      if(PhotonNetwork.IsConnected)
      {
        PhotonNetwork.JoinRandomRoom();
      }  
      else
      {
          PhotonNetwork.ConnectUsingSettings();
          PhotonNetwork.GameVersion = gameVersion;
      }

    }
    #endregion

    #region MonoBehaviorPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Launcher: ConnectedToMaster called");
        PhotonNetwork.JoinRandomRoom();
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
    }    


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnect Called by PUN with reason {0}", cause);
    }

    #endregion



}

}