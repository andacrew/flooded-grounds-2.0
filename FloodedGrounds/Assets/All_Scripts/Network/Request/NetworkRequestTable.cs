using UnityEngine;

using System;
using System.Collections.Generic;

public class NetworkRequestTable
{

    public static Dictionary<short, NetworkRequest> requestTable { get; set; }

    public static void init()
    {
        requestTable = new Dictionary<short, NetworkRequest>();

        add(Constants.CMSG_HEARTBEAT, new RequestHeartbeat());
        add(Constants.CMSG_PUSHUPDATE, new RequestPushUpdate());

        add(Constants.CMSG_REGISTER, new RequestRegister());
        add(Constants.CMSG_LOGIN, new RequestLogin());

        add(Constants.CMSG_GETLOBBIES, new RequestGetLobbies());
        add(Constants.CMSG_CREATELOBBY, new RequestCreateLobby());
        add(Constants.CMSG_JOINLOBBY, new RequestJoinGame());
        add(Constants.CMSG_STARTGAME, new RequestStartGame());
        add(Constants.CMSG_JOINGAME, new RequestJoinGame());

        add(Constants.CMSG_PICKUP, new RequestPickup());
    }

    public static void add(short request_id, NetworkRequest request)
    {
        requestTable.Add(request_id, request);
    }

    public static NetworkRequest get(short request_id)
    {
        NetworkRequest request = null;

        if (requestTable.ContainsKey(request_id))
            request = requestTable[request_id];
        else
            Debug.Log("Request [" + request_id + "] Not Found");

        return request;
    }
}