using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

public class TCPServer : MonoBehaviour
{
    [System.Serializable]
    public class MessageEvent : UnityEvent<JObject> { }
    [System.Serializable]
    public class MessageResponseEvent : UnityEvent<JObject, JObject> { }

    public string Address;
    public int Port;

    public UnityEvent OnConnectStart;
    public UnityEvent OnConnected;
    public UnityEvent OnConnectFailed;

    public MessageEvent OnMessageSend;
    public MessageEvent OnMessageSendFail;
    public MessageResponseEvent OnMessageSendComplete;

    public bool Connected
    {
        get
        {
            return mClient.Connected;
        }
    }

    private TcpClient mClient;

    private byte[] mDataBuffer;

    void Start()
    {
        mDataBuffer = new byte[4096];
        mClient = new TcpClient();
        Connect();
    }

    IEnumerator DoConnect()
    {
        OnConnectStart.Invoke();
        yield return new WaitForConnect(mClient, Address, Port);
        if (mClient.Connected)
            OnConnected.Invoke();
        else
            OnConnectFailed.Invoke();
    }

    IEnumerator DoSendMessage(JObject msg)
    {
        var data = msg.ToString(Formatting.None);

        mClient.Client.Send(Encoding.ASCII.GetBytes(data));
        OnMessageSend.Invoke(msg);

        yield return new WaitForReceive(mClient);

        if (mClient.Available == 0)
        {
            OnMessageSendFail.Invoke(msg);
            yield return null;
        }

        mClient.Client.Receive(mDataBuffer);
        data = Encoding.ASCII.GetString(mDataBuffer);
        var respMsg = JObject.Parse(data);

        OnMessageSendComplete.Invoke(msg, respMsg);
        yield return null;
    }

    public void Connect()
    {
        if(mClient.Connected)
            throw new System.Exception("Client is already connected to server");
        StartCoroutine(DoConnect());
    }

    public void CreateGame(string name)
    {
        var msg = new JObject();
        msg["type"] = (int)Protocol.BaseMessageType.CreateGameRequest;
        msg["name"] = name;
        StartCoroutine(DoSendMessage(msg));
    }

    public void Send(JObject message)
    {
        StartCoroutine(DoSendMessage(message));
    }
}