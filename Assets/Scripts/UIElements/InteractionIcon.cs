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

	public Transform[] slot;

	void Start()
	{
		HideIcons();
	}

	public void ShowIcons( string target, bool hand, bool eye, bool refl )
    {
		targetText.transform.GetComponent<Text>().text = target;
		targetText.SetActive( true );

		int iconNum = 0;
		//order: L->R: F->E->R
		if (refl)
        {
			reflImg.transform.SetParent(slot[iconNum], false);
			iconNum++;
		}
		if (hand)
		{
			handImg.transform.SetParent(slot[iconNum], false);
			iconNum++;
		}
		if (eye)
		{
			eyeImg.transform.SetParent(slot[iconNum], false);
		}

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
