using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ResponseJoinLobbyEventArgs : ExtendedEventArgs
{

    public short status { get; set; }
    public long user_id { get; set; }
    public int port { get; set; }

    public ResponseJoinLobbyEventArgs()
    {
        event_id = Constants.SMSG_JOINLOBBY;
    }
}

class ResponseJoinLobby : NetworkResponse
{
    private short status;
    private long user_id;
    private int port;

    public override void parse()
    {
        status = DataReader.ReadShort(dataStream);
        if (status == 0)
        {
            user_id = DataReader.ReadInt(dataStream);
            port = DataReader.ReadInt(dataStream);
        }

    }
    public override ExtendedEventArgs process()
    {
        ResponseJoinLobbyEventArgs args = null;
        if (status == 0)
        {
            args = new ResponseJoinLobbyEventArgs();
            args.status = status;
            args.user_id = user_id;
            args.port = port;
        }

        return args;
    }

}
