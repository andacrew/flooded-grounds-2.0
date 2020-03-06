using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsePickup : NetworkResponse
{
    private ConnectionManager connectionManager;

    override
    public void parse()
    {
    }

    override
    public ExtendedEventArgs process()
    {
        //Get the name of the object picked up
        string pickupItem = DataReader.ReadString(dataStream);

        //Disable the object
        GameObject.Find(pickupItem).SetActive(false);

        Debug.Log(pickupItem + " was picked up");

        return null;
    }
}