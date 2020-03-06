using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class RequestJoinLobby : NetworkRequest
{
    public RequestJoinLobby()
    {
        request_id = Constants.CMSG_JOINLOBBY;
    }

    public void send(long user_id, int port)
    {
        packet = new GamePacket(request_id);
        packet.addString(Constants.CLIENT_VERSION);
        packet.addLong64(user_id);
        packet.addInt32(port);
    }
}
