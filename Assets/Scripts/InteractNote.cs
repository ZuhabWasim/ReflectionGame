using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{
	public AudioClip millieResponse;
	
	protected override void OnUserInteract()
	{
		AudioPlayer.Play( Globals.AudioFiles.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
		// AudioPlayer.Play( voiceLine, Globals.Tags.DIALOGUE_SOURCE, false);
		AudioPlayer.Play( millieResponse, Globals.Tags.DIALOGUE_SOURCE, false);
	}

}
