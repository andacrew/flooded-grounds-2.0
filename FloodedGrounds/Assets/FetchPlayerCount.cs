 using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class FetchPlayerCount : MonoBehaviourPunCallbacks
{
Text waiting;
int playersInRoom = 1; 
    void Start()
    {
        waiting = this.GetComponent<Text>();
    }
    void update()
    {
        waiting.text = "Waiting on players...(" + playersInRoom + "/4).";
        Debug.Log(waiting.text);
        
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playersInRoom = PhotonNetwork.PlayerList.Length;
        Debug.Log("Player joined");
    }

}
