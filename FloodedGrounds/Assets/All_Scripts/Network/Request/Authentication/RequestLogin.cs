using UnityEngine;

using System;

public class RequestLogin : NetworkRequest {

	public RequestLogin() {
		request_id = Constants.CMSG_LOGIN;
	}
	
	public void send(string username, string password) {
	    packet = new GamePacket(request_id);
		packet.addString(Constants.CLIENT_VERSION);
		packet.addString(username);
		packet.addString(password);
        Debug.Log("Requested Login");
    }
}