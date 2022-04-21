using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenu;
	private bool isPaused;
	private bool objectiveOn;
	private bool subtitleOn;
	public GameObject objectives;
	private PlayerController m_playerController;
	void Start()
	{
		pauseMenu.SetActive( false );
		isPaused = false;
		objectiveOn = false;
		subtitleOn = true;
		m_playerController = GameObject.FindObjectOfType<PlayerController>();
	}
	public bool IsPaused()
	{
		return isPaused;
	}
	public void ToggleObjectives()
	{
		objectiveOn = !objectiveOn;
		objectives.SetActive( objectiveOn );
	}
	public void ToggleSubtitles()
	{
		subtitleOn = !subtitleOn;
		SubtitleDisplay.showSubs = subtitleOn;
	}
	public void SlideMouseSensitivity( float newValue )
	{
		m_playerController.SetSensitivity( newValue );
	}
	public void PauseGame()
	{
		isPaused = true;
		pauseMenu.SetActive( true );
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
	}
	public void ResumeGame()
	{
		isPaused = false;
		pauseMenu.SetActive( false );
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	public void ExitGame()
	{
		SceneManager.LoadScene( Globals.MENU_SCENE );
		EventManager.Fire( Globals.Events.GAME_RESTART );
		Cleanup();
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Confined;
	}

	private void Cleanup()
	{
		EventManager.OnExit(); // This needs to be called at the very end after other systems have shutdown
	}
}
