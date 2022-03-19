using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused;
    private int index;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }
    public bool IsPaused()
    {
        return isPaused;
    }
    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        index = InteractNote.pointer;
        showNote();
    }
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
	public void ExitGame()
	{
		SceneManager.LoadScene( 0 );
	}

    public void FlipRight()
    {
        if ( index <= InteractNote.journalSize - 2 )
        {
            index ++;
            showNote();
        }
    }
    public void FlipLeft()
    {
        if ( index >= 1 )
        {
            index --;
            showNote();
        }
    }

    private void showNote()
    {
        GameObject title = pauseMenu.transform.GetChild( 2 ).GetChild( 0 ).gameObject;
        GameObject content = pauseMenu.transform.GetChild( 2 ).GetChild( 1 ).gameObject;
        if ( InteractNote.journal[index] ) 
        {
            InteractNote note = InteractNote.journal[index];
            title.transform.GetComponent<Text>().text = note.title;
            content.transform.GetComponent<Text>().text = note.noteText.ToString();
        } 
        else
        {
            title.transform.GetComponent<Text>().text = Globals.UIStrings.NOTE_TITLE_PLACEHOLDER;
            content.transform.GetComponent<Text>().text = Globals.UIStrings.NOTE_PLACEHOLDER;
        }
        if ( index <= 0 )
        {
            pauseMenu.transform.GetChild( 4 ).gameObject.SetActive(false);
        }
        else {
            pauseMenu.transform.GetChild( 4 ).gameObject.SetActive(true);
        }
        if ( index >= InteractNote.journalSize - 1 )
        {
            pauseMenu.transform.GetChild( 3 ).gameObject.SetActive(false);
        }
        else {
            pauseMenu.transform.GetChild( 3 ).gameObject.SetActive(true);
        }
    }
}
