using UnityEngine;

using System;
using System.Collections.Generic;

public class NetworkResponseTable
{

    public static Dictionary<short, NetworkResponse> responseTable { get; set; }

    public static void init()
    {
        responseTable = new Dictionary<short, NetworkResponse>();
        responseTable.Add(Constants.SMSG_HEARTBEAT, new ResponseHeartbeat());
        responseTable.Add(Constants.SMSG_REGISTER, new ResponseRegister());
        responseTable.Add(Constants.SMSG_LOGIN, new ResponseLogin());

        responseTable.Add(Constants.SMSG_GETLOBBIES, new ResponseGetLobbies());
        responseTable.Add(Constants.SMSG_CREATELOBBY, new ResponseCreateLobby());
        responseTable.Add(Constants.SMSG_JOINLOBBY, new ResponseJoinLobby());
        responseTable.Add(Constants.SMSG_STARTGAME, new ResponseStartGame());
        responseTable.Add(Constants.SMSG_JOINGAME, new ResponseJoinGame());

        responseTable.Add(Constants.SMSG_PICKUP, new ResponsePickup());
        responseTable.Add(Constants.SMSG_HIT, new ResponseHit());
    }

    public static NetworkResponse get(short response_id)
    {
        NetworkResponse response = null;
        if (responseTable.ContainsKey(response_id))
            response = responseTable[response_id];
        else
            Debug.Log("Response [" + response_id + "] Not Found");

        return response;
    }
}