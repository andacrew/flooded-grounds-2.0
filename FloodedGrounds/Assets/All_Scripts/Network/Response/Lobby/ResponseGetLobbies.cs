using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ResponseGetLobbiesEventArgs : ExtendedEventArgs
{

    public short status { get; set; }
    public int size { get; set; }
    public Lobby[] lobbies { get; set; }

    public ResponseGetLobbiesEventArgs()
    {
        event_id = Constants.SMSG_GETLOBBIES;
    }
}


class ResponseGetLobbies : NetworkResponse
{
    private short status;
    private int size;
    private Lobby[] lobbies;

    public override void parse()
    {
        status = DataReader.ReadShort(dataStream);
        if (status == 0)
        {
            int port;
            string name;
            string ownerName;
            int privacy;
            bool passwordRequired;

            size = DataReader.ReadInt(dataStream);
            lobbies = new Lobby[size];
            for (int i = 0; i < size; i++)
            {
                port = DataReader.ReadInt(dataStream);
                name = DataReader.ReadString(dataStream);
                ownerName = DataReader.ReadString(dataStream);
                privacy = DataReader.ReadInt(dataStream);
                passwordRequired = DataReader.ReadBool(dataStream);
                lobbies[i] = new Lobby(port, name, ownerName, privacy, passwordRequired, "");
            }

        }
    }

    public override ExtendedEventArgs process()
    {
        ResponseGetLobbiesEventArgs args = null;
        if (status == 0)
        {
            args = new ResponseGetLobbiesEventArgs();
            args.lobbies = new Lobby[size];
            for (int i = 0; i < size; i++)
            {
                args.lobbies[i] = lobbies[i];
            }
        }

        return args;
    }
}

