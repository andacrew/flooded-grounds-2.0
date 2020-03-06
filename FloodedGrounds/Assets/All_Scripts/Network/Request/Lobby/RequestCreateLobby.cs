using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class RequestCreateLobby : NetworkRequest
{

    public RequestCreateLobby()
    {
        request_id = Constants.CMSG_CREATELOBBY;
    }

    public void send(Lobby lobby)
    {
        packet = new GamePacket(request_id);
        packet.addString(Constants.CLIENT_VERSION);
        packet.addString(lobby.getName());
        packet.addInt32(lobby.getPrivacy());
        packet.addBool(lobby.getPasswordRequired());
        packet.addString(lobby.getPassword());
        packet.addString(lobby.getOwnerName());
    }

}
