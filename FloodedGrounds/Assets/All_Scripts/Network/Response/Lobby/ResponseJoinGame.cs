using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseJoinGameEventArgs : ExtendedEventArgs
{
    public string character { get; set; }

    public ResponseJoinGameEventArgs()
    {
        event_id = Constants.SMSG_JOINGAME;
    }
}

public class ResponseJoinGame : NetworkResponse
{
    private string character;    

    override
    public void parse()
    {
        character = Constants.IDtoCharacter[DataReader.ReadShort(dataStream)];        
    }

    override
    public ExtendedEventArgs process()
    {
        ResponseJoinGameEventArgs args = new ResponseJoinGameEventArgs();
        args.character = character;
        return args;
    }
}