using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestRegister : NetworkRequest
{

    public RequestRegister()
    {
        request_id = Constants.CMSG_REGISTER;
    }

    public void send(string username, string email, string password)
    {
        packet = new GamePacket(request_id);
        packet.addString(Constants.CLIENT_VERSION);
        packet.addString(username);
        packet.addString(email);
        packet.addString(password);
    }
}
