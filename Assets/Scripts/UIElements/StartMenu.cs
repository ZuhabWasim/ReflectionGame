using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

	public void onClickStart()
	{
		SceneManager.LoadScene( Globals.MAIN_SCENE );
	}
	public void onClickQuit()
	{
		Application.Quit();
	}
}
