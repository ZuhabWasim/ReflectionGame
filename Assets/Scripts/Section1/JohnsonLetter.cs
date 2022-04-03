using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnsonLetter : InteractNote
{
	protected override void OnUserInteract()
	{
		base.OnUserInteract();
		AudioPlayer.Play( Globals.VoiceLines.Section1.JOHNSON_LETTER, Globals.Tags.DIALOGUE_SOURCE );
		AudioPlayer.Play(Globals.VoiceLines.Section1.MILLE_POV_INTRO, Globals.Tags.DIALOGUE_SOURCE, false);
		EventManager.Fire(Globals.Events.GO_ENTER_CODE, this.gameObject);
	}
}
