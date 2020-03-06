using UnityEngine;

using System;

public class RequestPickup : NetworkRequest
{
    private string pickupName;

    public RequestPickup()
    {
        request_id = Constants.CMSG_PICKUP;
    }

    public void setPickupName(string pickupName)
    {
        this.pickupName = pickupName;
    }

    public void send()
    {
        packet = new GamePacket(request_id);
        packet.addString(pickupName);
    }
}