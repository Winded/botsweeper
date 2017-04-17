using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Text;

public class TCPServer : MonoBehaviour
{
	public string Address;
	public int Port;

	private TcpClient mClient;

	IEnumerator CreateGame()
	{
		var msg = new Protocol.BaseMessage ();
		msg.type = Protocol.BaseMessageType.CreateGameRequest;
		msg.name = "My game";
		var data = JsonConvert.SerializeObject (msg);
		mClient.Client.Send (Encoding.Unicode.GetBytes(data));
		// TODO
		return null;
	}

	public void Connect()
	{
		mClient = new TcpClient ();
		mClient.Connect (Address, Port);
		StartCoroutine (CreateGame ());
	}
}