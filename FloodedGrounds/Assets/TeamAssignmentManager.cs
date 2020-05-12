
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class TeamAssignmentManager : MonoBehaviourPunCallbacks
{

[Tooltip("The level to be loaded after the sufficient playercount is met")]
[SerializeField]
string LoadLevel; 
PhotonTeam Human = new PhotonTeam();
Player[] humanCount = new Player[4];
PhotonTeam Monster = new PhotonTeam();


void Update(){

if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.GetPhotonTeam() == null)
{
    PhotonTeamsManager.Instance.TryGetTeamByName("Human", out Human);
    PhotonNetwork.LocalPlayer.JoinTeam(Human);
    PhotonTeamsManager.Instance.TryGetTeamMembers( 1 , out humanCount);

    Debug.Log("This client is on the team no. : " + PhotonNetwork.LocalPlayer.CustomProperties["_pt"]);
}
Debug.Log("(TeamAssignmentManager): is master? :" + PhotonNetwork.IsMasterClient );
Debug.Log ("Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
{
    int PlayerNum = Random.Range(0, PhotonNetwork.CurrentRoom.MaxPlayers - 1);
    PhotonTeamsManager.Instance.TryGetTeamByName("Monster", out Monster);
    PhotonNetwork.PlayerList[PlayerNum].SwitchTeam(Monster);
    Debug.Log("Switched Player no. " + PlayerNum + " to Monster");

    PhotonNetwork.AutomaticallySyncScene = false;
    PhotonNetwork.LoadLevel("New Game Scene");
    PhotonNetwork.AutomaticallySyncScene = true;
}

}

}
