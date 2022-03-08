using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEMP_IntroScreen : MonoBehaviour
{
	public RawImage img;

	public float introTime;

	private float currTime;
	private bool renderingImg;

	// Start is called before the first frame update
	void Start()
	{
		currTime = 0.0f;
		if ( introTime == 0f )
		{
			renderingImg = false;
		}
		else
		{
			img.gameObject.SetActive( true );
			renderingImg = true;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if ( renderingImg )
		{
			currTime += Time.deltaTime;
			float alpha = Mathf.Clamp( introTime - currTime, 0.0f, 1.0f );

			var color = img.color;
			color.a = alpha;
			img.color = color;

			if ( alpha == 0.0f )
			{
				renderingImg = false;
				img.gameObject.SetActive( false );
			}
		}
	}
}
