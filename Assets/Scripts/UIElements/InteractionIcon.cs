using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionIcon : MonoBehaviour
{
    
    public Texture reflectionImg;
    public Texture handImg;
    public Texture eyeImg;
    public RawImage iconImg;
    public GameObject interactionIcon;
    
    void Start()
    {
        interactionIcon.SetActive(false);
    }
    public void ShowReflectionIcon()
    {
        iconImg.texture = reflectionImg;
        interactionIcon.SetActive(true);
    }
    public void ShowEyeIcon()
    {
        iconImg.texture = eyeImg;
        interactionIcon.SetActive(true);
    }
    public void ShowHandIcon()
    {
        iconImg.texture = handImg;
        interactionIcon.SetActive(true);
    }
    public void HideIcon()
    {
        interactionIcon.SetActive(false);
    }
}
