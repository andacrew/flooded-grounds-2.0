using UnityEngine;
using System;

public class RequestStartGame : NetworkRequest
{
    public RequestStartGame()
    {
        request_id = Constants.CMSG_STARTGAME;
    }

    public void send()
    {
        packet = new GamePacket(request_id);
        Debug.Log("Requested Start Game");
    }
}