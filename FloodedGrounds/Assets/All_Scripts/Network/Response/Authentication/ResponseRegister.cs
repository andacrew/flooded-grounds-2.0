using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResponseRegisterEventArgs : ExtendedEventArgs
{

    public short status { get; set; }
    public int user_id { get; set; }
    public string username { get; set; }

    public ResponseRegisterEventArgs()
    {
        event_id = Constants.SMSG_REGISTER;
    }
}

public class ResponseRegister : NetworkResponse
{

    private short status;
    private int user_id;
    private string username;

    public ResponseRegister()
    {
    }

    public override void parse()
    {
        status = DataReader.ReadShort(dataStream);
        if (status == 0)
        {
            user_id = DataReader.ReadInt(dataStream);
            username = DataReader.ReadString(dataStream);
        }
    }

    public override ExtendedEventArgs process()
    {
        ResponseRegisterEventArgs args = null;
        if (status == 0)
        {
            args = new ResponseRegisterEventArgs();
            args.status = status;
            args.user_id = user_id;
            args.username = username;
        }
        else
        {
            Debug.Log("Registration failed:");
            if (status == 1) Debug.Log("Email already in use");
            else if (status == 2) Debug.Log("Username already in use");
        }

        return args;
    }
}