using System;
using UnityEngine;

class RequestKeepAlive : NetworkRequest
{
    public RequestKeepAlive()
    {
        request_id = Constants.CMSG_KEEPALIVE;
    }

    public void send()
    {
        packet = new GamePacket(request_id);
    }
}
