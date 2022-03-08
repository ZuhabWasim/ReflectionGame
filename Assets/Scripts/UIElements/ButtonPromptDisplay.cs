using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptDisplay : MonoBehaviour
{

	public GameObject promptHolder;
	public Text promptText;
	public RawImage buttonImg;

	public char[] buttonChar;
	public Texture[] buttonTexture;
	//public Dictionary<char, Texture> buttonTextures;

	public float btnTextSpace;

	// Start is called before the first frame update
	void Start()
	{
		promptHolder.SetActive( false );
		updatePromptPos();
	}

	public void SetButton( char c )
	{
		int i;
		int k = -1;
		for ( i = 0; i < buttonChar.Length; i++ )
		{
			if ( buttonChar[ i ] == c )
			{
				k = i;
				break;
			}
		}
		if ( k >= 0 )
		{
			buttonImg.texture = buttonTexture[ k ];
		}
	}

	public void showPrompt( string t )
	{
		promptText.text = t;
		updatePromptPos();
		promptHolder.SetActive( true );
	}

	public void hidePrompt()
	{
		promptHolder.SetActive( false );
	}

	private void updatePromptPos()
	{
		promptText.transform.localPosition = new Vector3( ( buttonImg.rectTransform.rect.width + btnTextSpace ) / 2,
			promptText.transform.localPosition.y, 0 );
		buttonImg.transform.localPosition = new Vector3( -( promptText.preferredWidth + btnTextSpace ) / 2,
			buttonImg.transform.localPosition.y, 0 );
	}

}
