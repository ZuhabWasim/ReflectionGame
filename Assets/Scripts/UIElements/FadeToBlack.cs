using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour
{
	public Image img;
	public float defFadeTime;
	public float fadeToMainAfterTime = 0;

	private float totalTime;
	private float currTime;
	private bool fadingIn;
	private bool fadingOut;

	private float startTime;

	private static System.Action FadeOutFunc;

	// Start is called before the first frame update
	void Start()
	{
		startTime = Time.time;
		StartFadeIn();
	}

	// Update is called once per frame
	void Update()
	{
		if (fadeToMainAfterTime != 0 && Time.time - startTime > fadeToMainAfterTime)
        {
			Debug.Log("REACHED FADEOUT");
			fadeToMainAfterTime = 0;
			StartFadeOut(FadeToMain);
        }
		if (fadingIn)
		{
			currTime -= Time.deltaTime;
			float alpha = Mathf.Clamp(currTime/totalTime, 0.0f, 1.0f);

			var color = img.color;
			color.a = alpha;
			img.color = color;

			if (alpha == 0.0f)
			{
				fadingIn = false;
				img.gameObject.SetActive(false);
			}
		} else if (fadingOut)
        {
			currTime -= Time.deltaTime;
			float alpha = Mathf.Clamp(1 - (currTime / totalTime), 0.0f, 1.0f);

			var color = img.color;
			color.a = alpha;
			img.color = color;

			if (alpha == 1.0f)
			{
				fadingOut = false;
				FadeOutFunc();
			}
		}
	}

	void FadeToMain()
    {
		SceneManager.LoadScene(Globals.MENU_SCENE);
		EventManager.Fire(Globals.Events.GAME_RESTART);
		EventManager.OnExit();
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Confined;
	}

	public void StartFadeOut(System.Action callback, float dur = -1.0f)
    {
		FadeOutFunc = callback;
		if (dur == -1.0f)
		{
			dur = defFadeTime;
		}
		totalTime = dur;
		currTime = dur;
		if (dur == 0f)
		{
			fadingIn = false;
			fadingOut = false;
			FadeOutFunc();
		}
		else
		{
			img.gameObject.SetActive(true);
			fadingIn = false;
			fadingOut = true;
		}
	}

	public void StartFadeIn(float dur = -1.0f)
    {
		if (dur == -1.0f)
        {
			dur = defFadeTime;
        }
		totalTime = dur;
		currTime = dur;
		if (dur == 0f)
		{
			fadingIn = false;
			fadingOut = false;
		}
		else
		{
			img.gameObject.SetActive(true);
			fadingIn = true;
			fadingOut = false;
		}
	}
}
