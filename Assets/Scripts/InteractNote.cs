using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{

	public int id;
	public static InteractNote[] journal = new InteractNote[journalSize];;
	public static int journalSize = 5;
	void start()
	{
		journal[id] = this;
	}
	protected override void OnUserInteract()
	{
		AudioPlayer.Play( Globals.AudioFiles.General.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
		AudioPlayer.Play( Globals.VoiceLines.Section1.MILLE_POV_INTRO, Globals.Tags.DIALOGUE_SOURCE, false);
	}

}
