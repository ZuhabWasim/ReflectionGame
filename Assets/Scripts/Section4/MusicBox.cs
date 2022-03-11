using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : InteractableAbstract
{

	private const string MUSICBOX_AUDIO_SOURCE = "MusicBoxAudioSource";
	private Inventory m_inventory;
	public AudioClip millieKeyVoiceline;
	public AudioClip motherKeyVoiceline;
	public AudioClip fatherKeyVoiceline;
	public bool discoveredBox;
	
	protected override void OnStart()
	{
		//EventManager.Sub( Globals.Events.MILLIE_KEY_INTERACT, MillieKeyInteract );
		EventManager.Sub( Globals.Events.HAS_MILLIE_KEY, OnHavingMillieKey );

		m_inventory = Inventory.GetInstance();
		// AudioPlayer.RegisterAudioPlayer(MUSICBOX_AUDIO_SOURCE, GetComponent<AudioSource>());

		discoveredBox = false;
		desiredItem = Globals.UIStrings.MUSICBOXMILLIE_ITEM;
	}

	protected override void OnUserInteract()
	{
		AudioPlayer.Play( voiceLine, Globals.Tags.DIALOGUE_SOURCE );
		discoveredBox = true;
	}

	protected override void OnUseItem()
	{
		Debug.Log( "Initially interacting with the Music Box with Millie's key" );
		HandleInteract();
	}

	void HandleInteract()
	{
		MillieKeyInteract();
	}

	void MillieKeyInteract()
	{
		AudioPlayer.Play( millieKeyVoiceline, Globals.Tags.DIALOGUE_SOURCE );
		EventManager.Fire( Globals.Events.MILLIE_KEY_INTERACT );
	}

	private bool HasMillieKey()
	{
		return m_inventory.HasItem(Globals.UIStrings.MUSICBOXMILLIE_ITEM);
	}
	
	void OnHavingMillieKey()
	{
		if ( HasMillieKey() )
		{
			AudioPlayer.Play( Globals.VoiceLines.Section1.DISCOVER_KEY_FIRST__OH_THERE_IT_IS, Globals.Tags.DIALOGUE_SOURCE );
		}
	}
}
