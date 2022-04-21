using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

	public void onClickStart()
	{
		GameObject.Find(Globals.Misc.Canvas).GetComponent<FadeToBlack>().StartFadeOut(onFadeOut);
	}
	public void onClickCredits()
	{
		Application.OpenURL("https://zuhab.itch.io/reflection");
	}
	public void onFadeOut()
    {
		SceneManager.LoadScene(Globals.MAIN_SCENE);
	}
	public void onClickQuit()
	{
		Application.Quit();
	}
}
