using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillieKey : PickupItem
{
	void Start()
	{
		base.OnStart();
	}
	protected override void OnUserInteract()
	{
		this.gameObject.SetActive( false );

		MusicBox musicBox = GameObject.FindGameObjectWithTag( Globals.Tags.MUSIC_BOX ).GetComponent<MusicBox>();
		if ( musicBox.discoveredBox )
		{
			AudioPlayer.Play( Globals.VoiceLines.Section1.DISCOVER_BOX_FIRST, Globals.Tags.DIALOGUE_SOURCE );
		}
		else
		{
			AudioPlayer.Play( voiceLine, Globals.Tags.DIALOGUE_SOURCE );
		}
		EventManager.Fire(Globals.Events.HAS_MILLIE_KEY);
	}
}
