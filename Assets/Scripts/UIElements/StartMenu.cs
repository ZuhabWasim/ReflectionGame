using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

	public void onClickStart()
	{
		SceneManager.LoadScene( 1 );
	}
	public void onClickQuit()
	{
		Application.Quit();
	}
}
