using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mine : MonoBehaviour
{
    public Color NeutralColor;
    public Color GoodColor;
    public Color BadColor;

    public Image CurrentColorImage;
    public Image NewColorImage;

    private Animator mAnimator;

    public void SwitchColorStart(bool good)
    {
        NewColorImage.color = good ? GoodColor : BadColor;
        mAnimator.Play("Change");
    }

    public void SwitchColorEnd()
    {
        CurrentColorImage.color = NewColorImage.color;
    }

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }
}