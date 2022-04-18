using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{
	public static List<InteractNote> journal = new List<InteractNote>();
	public static int pointer = 0;
	
	public string title;
	public Texture noteText;
	
	public AudioClip letterAudio;
	public AudioClip responseAudio;

	public string onReadFireEvent = "";
	
	[Tooltip( "Whether the player will hear Millie's response to the letter when playing back." )]
	public bool playbackResponse = false;
	public static void InitJournal()
	{
		journal = new List<InteractNote>();
	}
	public override void OnUseItemUnfiltered()
	{
		AddNote();
		
		AudioPlayer.Play( Globals.AudioFiles.General.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
		AudioPlayer.Play( letterAudio, Globals.Tags.DIALOGUE_SOURCE );
		AudioPlayer.Play( responseAudio, Globals.Tags.DIALOGUE_SOURCE, false );
		if (onReadFireEvent != string.Empty)
		{
			EventManager.Fire( onReadFireEvent );
		}
		EventManager.Fire( InputManager.GetKeyDownEventName( Keybinds.TAB_KEY ) );
	}

	public void PlaybackAudio()
	{
		AudioPlayer.Play( letterAudio, Globals.Tags.DIALOGUE_SOURCE );
		if (playbackResponse)
		{
			AudioPlayer.Play( responseAudio, Globals.Tags.DIALOGUE_SOURCE, false );
		}
	}
	
	public void AddNote()
	{
		if ( pointer == 0 )
		{
			journal.Add( null );
		} 
		journal.Add(this);
		this.gameObject.SetActive( false );
		pointer += 1;
	}

	public static void OnExit()
	{
		pointer = 0;
		journal.Clear();
	}
}
