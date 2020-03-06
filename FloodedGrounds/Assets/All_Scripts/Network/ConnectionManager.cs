using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

public class ConnectionManager : MonoBehaviour
{	
	private GameObject mainObject;
	private TcpClient mySocket;
	private NetworkStream theStream;
	private bool socketReady = false;
    //The packet number of the last update packet processed
    private int latestUpdateNumber = -1;
	
	void Awake() {
		mainObject = GameObject.Find("MainObject");
	}
	
	// Use this for initialization
	void Start () {
        setupSocket ();
    }

	public void setupSocket() {
		if (socketReady) {
			Debug.Log("Already Connected");
			return;
		}
		try {
			mySocket = new TcpClient (Constants.REMOTE_HOST, Constants.REMOTE_PORT);
			theStream = mySocket.GetStream();
			socketReady = true;
			Debug.Log("Connected");
		} catch (Exception e) {
			Debug.Log("Socket error: " + e);
		}
	}

	public void readSocket() {
		if (!socketReady) {
			return;
		}
		if (theStream != null && theStream.DataAvailable) {
			byte[] buffer = new byte[2];
			theStream.Read(buffer, 0, 2);
			short bufferSize = BitConverter.ToInt16(buffer, 0);
			buffer = new byte[bufferSize];
			theStream.Read(buffer, 0, bufferSize);
			MemoryStream dataStream = new MemoryStream(buffer);
			short response_id = DataReader.ReadShort(dataStream);
			NetworkResponse response = NetworkResponseTable.get(response_id);
			if (response != null) {
				response.dataStream = dataStream;
				response.parse();
				ExtendedEventArgs args = response.process();
                if (args != null)
                {
                    MessageQueue msgQueue = mainObject.GetComponent<MessageQueue>();
                    msgQueue.AddMessage(args.event_id, args);
                }
            }
		}
	}

	public void closeSocket() {
		if (!socketReady) {
			return;
		}
		mySocket.Close();
		socketReady = false;
	}
	
	public void send(NetworkRequest request) {
		if (!socketReady) {
			return;
		}
		GamePacket packet = request.packet;
		byte[] bytes = packet.getBytes();
		theStream.Write(bytes, 0, bytes.Length);
	}
	
	// Update is called once per frame
	void Update () {
        while(theStream != null && theStream.DataAvailable)
		    readSocket();
	}

    public void setUpdateNumber(int updateNumber)
    {
        latestUpdateNumber = updateNumber;
    }

    public int getUpdateNumber()
    {
        return latestUpdateNumber;
    }
}