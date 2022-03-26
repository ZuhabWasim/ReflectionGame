using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionIcon : MonoBehaviour
{

	public GameObject handImg;
	public GameObject eyeImg;
	public GameObject reflImg;

	void Start()
	{
		HideIcons();
	}

	public void ShowIcons( bool hand, bool eye, bool refl )
    {
		handImg.SetActive( hand );
		eyeImg.SetActive( eye );
		reflImg.SetActive( refl );
	}

	public void HideIcons()
	{
		handImg.SetActive( false );
		eyeImg.SetActive( false );
		reflImg.SetActive( false );
	}
}
