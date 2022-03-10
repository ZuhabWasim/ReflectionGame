using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHandkerchief : InteractableAbstract
{
	private const string MIRROR_AUDIO_SOURCE = "MirrorAudioSource";
	public AudioClip soundEffect;
	public AudioClip cleanMirrorVoiceline;
	public AudioClip afterCleanVoiceline;

	private MirrorPlane m_dirtyMirror; // TODO(dennis): remove this, this is utterly imbecilic
	private Inventory m_inventory;
	private bool mirrorCleaned;

	protected override void OnStart()
	{
		m_inventory = Inventory.GetInstance();
		AudioPlayer.RegisterAudioPlayer( MIRROR_AUDIO_SOURCE, GetComponent<AudioSource>() );

		mirrorCleaned = false;
		desiredItem = Globals.UIStrings.HANDKERCHIEF_ITEM;

		m_dirtyMirror = gameObject.GetComponent<MirrorPlane>();
	}

	protected override void OnUserInteract()
	{
		if ( mirrorCleaned )
		{
			AudioPlayer.Play( afterCleanVoiceline, Globals.Tags.DIALOGUE_SOURCE );
		}
		else
		{
			AudioPlayer.Play( Globals.AudioFiles.DIRTY_MIRROR, Globals.Tags.DIALOGUE_SOURCE );
		}
	}

	protected override void OnUseItem()
	{
		m_inventory.DeleteItem( desiredItem );
		HandleUseItem();
	}

	void HandleUseItem()
	{
		// We can probably keep the above function as inheritable and do all specific things like changing the dirty texture here
		AudioPlayer.Play( soundEffect, MIRROR_AUDIO_SOURCE );
		AudioPlayer.Play( cleanMirrorVoiceline, Globals.Tags.DIALOGUE_SOURCE );

		if ( m_dirtyMirror != null )
		{
			m_dirtyMirror.CleanMirror();
			mirrorCleaned = true;
		}
	}
}
