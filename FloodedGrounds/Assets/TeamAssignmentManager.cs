
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class TeamAssignmentManager : MonoBehaviourPunCallbacks
{
    PhotonTeam Human; 
    PhotonTeam Monster; 

    void Start(){
        PhotonTeamsManager.Instance.TryGetTeamByCode(1, out Human);
        PhotonTeamsManager.Instance.TryGetTeamByCode(2, out Monster);
    }
    void Update()
    {
        
    }
}
