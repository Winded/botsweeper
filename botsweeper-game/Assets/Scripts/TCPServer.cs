using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Text;

public class TCPServer : MonoBehaviour
{
    [System.Serializable]
    public class MessageEvent : UnityEvent<Protocol.Message> { }
    [System.Serializable]
    public class MessageResponseEvent : UnityEvent<Protocol.Message, Protocol.Message> { }

    public string Address;
    public int Port;

    public MessageEvent OnMessageSend;
    public MessageEvent OnMessageSendFail;
    public MessageResponseEvent OnMessageSendComplete;

    private TcpClient mClient;

    private byte[] mDataBuffer;

    void Start()
    {
        mDataBuffer = new byte[4096];
    }

    IEnumerator CreateGame()
    {
        var msg = new Protocol.Message();
        msg["type"] = Protocol.BaseMessageType.CreateGameRequest;
        msg["name"] = "My game";

        var data = JsonConvert.SerializeObject(msg);
        print(data);
        mClient.Client.Send(Encoding.ASCII.GetBytes(data));
        OnMessageSend.Invoke(msg);

        yield return new WaitForReceive(mClient);

        if(mClient.Available == 0)
        {
            OnMessageSendFail.Invoke(msg);
            yield return null;
        }

        mClient.Client.Receive(mDataBuffer);
        data = Encoding.ASCII.GetString(mDataBuffer);
        var respMsg = JsonConvert.DeserializeObject<Protocol.Message>(data);

        OnMessageSendComplete.Invoke(msg, respMsg);
        yield return null;
    }

    public void Connect()
    {
        mClient = new TcpClient();
        mClient.Connect(Address, Port);
        StartCoroutine(CreateGame());
    }
}