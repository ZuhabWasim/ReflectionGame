using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalDisplay : MonoBehaviour
{
	public GameObject journal;
    private bool isOpened;
    private int index;
    void Start()
    {
        journal.SetActive( false );
    }
    public bool IsOpened()
    {
        return isOpened;
    }
    public void OpenJournal()
    {
        isOpened = true;
        journal.SetActive( true );
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        index = InteractNote.pointer / 2;
        ShowPage();
    }
    public void CloseJournal()
    {
        isOpened = false;
        journal.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void FlipRight()
    {
        if ( index <= InteractNote.pointer / 2 - 1 )
        {
            index ++;
            ShowPage();
        }
    }
    public void FlipLeft()
    {
        if ( index >= 1 )
        {
            index --;
            ShowPage();
        }
    }
    private void ShowNote(GameObject page, InteractNote note)
    {
        GameObject title = page.transform.GetChild( 1 ).gameObject;
        GameObject content = page.transform.GetChild( 0 ).gameObject;
        title.transform.GetComponent<Text>().text = note.title;
        content.transform.GetComponent<RawImage>().texture = note.noteText;
    }
    public void PlayAudioLeft()
    {
        int leftId = index * 2;
        AudioClip letterAudio = InteractNote.journal[leftId].letterAudio;
		AudioPlayer.Play( letterAudio, Globals.Tags.DIALOGUE_SOURCE );
    }
    public void PlayAudioRight()
    {
        int rightId = index * 2 + 1;
        AudioClip letterAudio = InteractNote.journal[rightId].letterAudio;
		AudioPlayer.Play( letterAudio, Globals.Tags.DIALOGUE_SOURCE );
    }
    private void ShowPage()
    {
        int leftId = index * 2;
        int rightId = index * 2 + 1;
        GameObject leftPage = journal.transform.GetChild( 1 ).gameObject;
        GameObject rightPage = journal.transform.GetChild( 2 ).gameObject;
        if ( leftId == 0 ) 
        {
            leftPage.SetActive( false );
        }
        else
        {
            ShowNote(leftPage, InteractNote.journal[leftId]);
            leftPage.SetActive( true );
        }
        if ( rightId > InteractNote.pointer)
        {
            rightPage.SetActive( false );
        }
        else
        {
            ShowNote(rightPage, InteractNote.journal[rightId]);
            rightPage.SetActive( true );
        }
        GameObject leftIndex = journal.transform.GetChild( 3 ).gameObject;
        GameObject rightIndex = journal.transform.GetChild( 4 ).gameObject;
        leftIndex.transform.GetComponent<Text>().text = leftId.ToString();
        rightIndex.transform.GetComponent<Text>().text = rightId.ToString();
        GameObject leftButton = journal.transform.GetChild( 5 ).gameObject;
        GameObject rightButton = journal.transform.GetChild( 6 ).gameObject;
        if ( index >= 1 )
        {
            leftButton.SetActive( true );
        }
        else
        {
            leftButton.SetActive( false );
        }
        if ( index <= InteractNote.pointer / 2 - 1 )
        {
            rightButton.SetActive( true );
        }
        else
        {
            rightButton.SetActive( false );
        }
    }
}
