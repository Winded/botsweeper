using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class UILoadingScreen : MonoBehaviour
{
    public Text LoadingText;

    private UIContainer mContainer;

    void Start()
    {
        mContainer = GetComponent<UIContainer>();
    }

    public void Show(string text)
    {
        LoadingText.text = text;
        mContainer.Enable();
    }

    public void Hide()
    {
        mContainer.Disable();
    }

    public void OnMessageSend(JObject message)
    {
        var type = (Protocol.BaseMessageType)(int)message["type"];
        if (type == Protocol.BaseMessageType.CreateGameRequest)
        {
            Show("Creating game");
        }
    }

    public void OnMessageSendComplete(JObject message, JObject response)
    {
        Hide();
    }
}
