using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIContainer : MonoBehaviour
{
    public bool initiallyEnabled = false;

    private CanvasGroup mCanvasGroup;
    private bool mEnabled;

    public bool isEnabled
    {
        get
        {
            return mEnabled;
        }
    }

    public void Enable()
    {
        mCanvasGroup.alpha = 1f;
        mCanvasGroup.interactable = true;
        mCanvasGroup.blocksRaycasts = true;
        mEnabled = true;
    }

    public void Disable()
    {
        mCanvasGroup.alpha = 0f;
        mCanvasGroup.interactable = false;
        mCanvasGroup.blocksRaycasts = false;
        mEnabled = false;
    }

    void Start()
    {
        mCanvasGroup = GetComponent<CanvasGroup>();
        if (initiallyEnabled)
            Enable();
        else
            Disable();
    }
}