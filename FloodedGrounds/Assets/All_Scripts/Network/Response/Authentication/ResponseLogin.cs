using UnityEngine;

using System;

public class ResponseLoginEventArgs : ExtendedEventArgs {

    public short status { get; set; }
    public long user_id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public int gamesPlayed { get; set; }
    public int gamesWon { get; set; }
    public int gamesLost { get; set; }

    public ResponseLoginEventArgs() {
		event_id = Constants.SMSG_LOGIN;
	}
}

public class ResponseLogin : NetworkResponse {

    private short status;
    private long user_id;
    private string username;
    private string email;
    private int gamesPlayed;
    private int gamesWon;
    private int gamesLost;

    public ResponseLogin() {
	}
	
	public override void parse() {
		status = DataReader.ReadShort(dataStream);
		if (status == 0) {
			user_id = DataReader.ReadLong(dataStream);
			username = DataReader.ReadString(dataStream);
            email = DataReader.ReadString(dataStream);
            gamesPlayed = DataReader.ReadInt(dataStream);
            gamesWon = DataReader.ReadInt(dataStream);
            gamesLost = DataReader.ReadInt(dataStream);
        }
	}
	
	public override ExtendedEventArgs process() {
		ResponseLoginEventArgs args = null;
		if (status == 0) {
            // Main.setLoggedIn(true);
            args = new ResponseLoginEventArgs();
			args.status = status;
			args.user_id = user_id;
			args.username = username;
            args.email = email;
            args.gamesPlayed = gamesPlayed;
            args.gamesWon = gamesWon;
            args.gamesLost = gamesLost;
            Debug.Log("Logged In");
        }
        else
            Debug.Log("Login Failed");

        return args;
	}
}