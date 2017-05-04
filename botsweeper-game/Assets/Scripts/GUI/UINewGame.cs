using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINewGame : MonoBehaviour
{
    public InputField NameField;

    private UIContainer mContainer;

    void Start()
    {
        mContainer = GetComponent<UIContainer>();
    }

    public void OnMessageSend(JObject message)
    {
        if ((Protocol.BaseMessageType)(int)message["type"] == Protocol.BaseMessageType.CreateGameRequest)
        {
            mContainer.Disable();
        }
    }

    public void OnMessageSendComplete(JObject message, JObject response)
    {
        // Do nuffin
    }

    public void CreateGame()
    {
        var go = GameObject.FindGameObjectWithTag("GameManager");
        var s = go.GetComponent<TCPServer>();
        s.CreateGame(NameField.text);
    }
}