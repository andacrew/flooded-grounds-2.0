using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun; 
using Photon.Realtime;
using Photon.Pun.UtilityScripts;


public class FloodedGroundsGameLogic : MonoBehaviour
{

[Tooltip("The Keycard Objects in Scene to be collected by the human team")]
[SerializeField]


PhotonTeam Human; 
PhotonTeam Monster; 
Player[] monsterTeamArr; 

public GameObject[] keyCards = new GameObject[3];
int activeKeys; 
public static bool humanWin; 

    void Start()
    {
        foreach (GameObject key in keyCards)
        {
            key.SetActive(true);
        }

        PhotonTeamsManager.Instance.TryGetTeamByCode(1, out Human);
        PhotonTeamsManager.Instance.TryGetTeamByCode(2, out Monster);
    }

    
    void LateUpdate()
    {
        checkTeams();
        checkWin();
        
    }

    private void checkTeams()
    {
        if(PhotonNetwork.LocalPlayer.GetPhotonTeam() == null)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                PhotonNetwork.LocalPlayer.JoinTeam(2);
            } else 
            {
                PhotonNetwork.LocalPlayer.JoinTeam(1);
            }
        }
    }

    private void checkWin()
    {
        activeKeys = keyCards.Count(); 
        foreach (GameObject key in keyCards)
        {
            activeKeys = key.activeInHierarchy ? activeKeys : activeKeys-1; 
        }
        humanWin = activeKeys == 0 ? true : false; 
        PhotonTeamsManager.Instance.TryGetTeamMembers(2, out monsterTeamArr);
        Debug.Log("GameOver? :: " + humanWin);

    }
}
