using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINewGame : MonoBehaviour
{
    public UIContainer WaitText;

    public InputField NameField;

    public void OnMessageSend(Protocol.Message message)
    {
        if ((Protocol.BaseMessageType)message["type"] == Protocol.BaseMessageType.CreateGameRequest)
        {
            WaitText.Enable();
            NameField.interactable = false;
        }
    }

    public void OnMessageSendComplete(Protocol.Message message, Protocol.Message response)
    {
        if ((Protocol.BaseMessageType)message["type"] == Protocol.BaseMessageType.CreateGameRequest)
        {
            WaitText.Disable();
            NameField.interactable = true;
            var game = (Dictionary<object, object>)response["game"];
            var id = (string)game["id"];
            print(id);
        }
    }
}