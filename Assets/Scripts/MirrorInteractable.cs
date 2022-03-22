using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Default mirror interactable behaviours
public class MirrorInteractable : InteractableAbstract
{
	[Tooltip( "Name of the audio source. If unset, will fall back to GameObject.name" )]
	public string audioSourceName;

	public bool teleportable;
	public string makeTeleportableEvent;
	public string makeNonTeleportableEvent;
	public AudioClip teleportableVoiceLine;
	public AudioClip nonTeleportableVoiceLine;
	public AudioClip teleportingVoiceLine;

	private MirrorConnector m_connector;
	private string m_audioSourceName;

	// Start is called before the first frame update
	protected override void OnStart()
	{
		thisIsAMirror = true;
		if ( GetComponent<AudioSource>() != null )
		{
			m_audioSourceName = audioSourceName == "" ? this.name : audioSourceName;
			AudioPlayer.RegisterAudioPlayer( m_audioSourceName, GetComponent<AudioSource>() );
		}

		if ( makeTeleportableEvent != string.Empty )
		{
			EventManager.Sub( makeTeleportableEvent, () => { teleportable = true; } );
		}

		if ( makeNonTeleportableEvent != string.Empty )
		{
			EventManager.Sub( makeNonTeleportableEvent, () => { teleportable = false; } );
		}

		// IF PENGUINS ARE SO SMART, HOW COME THEY LIVE IN IGLOOS?
	}

	public bool getTeleportable()
	{
		return teleportable;
	}

	public void setTeleportable( bool teleportable )
	{
		this.teleportable = teleportable;
	}

	protected override void OnUserInteract()
	{
		HandleInteract();
	}

	private void HandleInteract()
	{
		if ( teleportable )
		{
			AudioPlayer.Play( teleportableVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
		}
		else
		{
			// We're going to use the regular voiceline from Interactable as any dialog when the mirror is not teleportable.
			AudioPlayer.Play( voiceLine, Globals.Tags.DIALOGUE_SOURCE );
		}
	}
	
	public void SetMirrorConnector( MirrorConnector connector )
	{
		m_connector = connector;
	}
	
	public override void OnUseItemUnfiltered()
	{
		// Only Teleports if the Mirror Interactable allows it and the Mirror is interactable to begin with.
		if ( interactable && teleportable )
		{
			m_connector.HandleUserTeleport();
			AudioPlayer.Play( teleportingVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
			// Also play reflecting sound
		}
		else
		{
			AudioPlayer.Play( nonTeleportableVoiceLine, Globals.Tags.DIALOGUE_SOURCE );
		}
	}
}
