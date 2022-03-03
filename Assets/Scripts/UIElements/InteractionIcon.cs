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
    public void showReflectionIcon()
    {
        iconImg.texture = reflectionImg;
        interactionIcon.SetActive(true);
    }
    public void showEyeIcon()
    {
        iconImg.texture = eyeImg;
        interactionIcon.SetActive(true);
    }
    public void showHandIcon()
    {
        iconImg.texture = handImg;
        interactionIcon.SetActive(true);
    }
    public void hideIcon()
    {
        interactionIcon.SetActive(false);
    }
}
