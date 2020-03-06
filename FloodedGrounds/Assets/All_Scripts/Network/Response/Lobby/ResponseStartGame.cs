using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseStartGame : NetworkResponse
{
    override
    public void parse()
    {

    }

    override
    public ExtendedEventArgs process()
    {
        //Debug.Log("Game Started");
        //Debug.Log("Failed To Start Game");
        return null;
    }
}