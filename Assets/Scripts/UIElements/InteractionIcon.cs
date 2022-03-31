using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionIcon : MonoBehaviour
{

	public GameObject handImg;
	public GameObject eyeImg;
	public GameObject reflImg;
	public GameObject targetText;

	void Start()
	{
		HideIcons();
	}

	public void ShowIcons( string target, bool hand, bool eye, bool refl )
    {
		targetText.transform.GetComponent<Text>().text = target;
		targetText.SetActive( true );
		handImg.SetActive( hand );
		eyeImg.SetActive( eye );
		reflImg.SetActive( refl );
	}

	public void HideIcons()
	{
		targetText.SetActive( false );
		handImg.SetActive( false );
		eyeImg.SetActive( false );
		reflImg.SetActive( false );
	}
}
