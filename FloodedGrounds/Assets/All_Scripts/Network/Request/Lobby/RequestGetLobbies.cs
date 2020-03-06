using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RequestGetLobbies : NetworkRequest {
    public RequestGetLobbies() {
        request_id = Constants.CMSG_GETLOBBIES;
    }

    public void send() {
        packet = new GamePacket(request_id);
        Debug.Log("Requested Lobby List");
    }

}

