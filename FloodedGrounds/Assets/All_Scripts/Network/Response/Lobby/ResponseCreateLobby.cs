using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ResponseCreateLobbyEventArgs : ExtendedEventArgs
{
    public short status { get; set; }
    public int port { get; set; }

    public ResponseCreateLobbyEventArgs()
    {
        event_id = Constants.SMSG_CREATELOBBY;
    }
}


class ResponseCreateLobby : NetworkResponse
{
    private short status;
    private int port;

    public override void parse()
    {
        status = DataReader.ReadShort(dataStream);
        if (status == 0)
            port = DataReader.ReadInt(dataStream);
    }

    public override ExtendedEventArgs process()
    {
        ResponseCreateLobbyEventArgs args = null;
        if (status == 0)
        {
            args = new ResponseCreateLobbyEventArgs();
            args.status = status;
            args.port = port;
        }

        return args;
    }
}
