using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateInteractableWrapper : InteractableAbstract
{
	//THIS OBJECT MUST ALSO HAVE AN INTERPOLATETRANSFORM SCRIPT

	private InterpolateTransform it;
	public AudioClip openSound;
	public AudioClip closeSound;
	public string audioSourceName;

	private string m_audioSource;
	
	protected override void OnStart()
	{
		it = GetComponent<InterpolateTransform>();
		if (GetComponent<AudioSource>() != null)
		{
			m_audioSource = audioSourceName == "" ? this.name : audioSourceName;
			AudioPlayer.RegisterAudioPlayer( m_audioSource, GetComponent<AudioSource>() );
		}
	}

	protected override void OnUserInteract()
	{
		if ( it.ActivateInteractMotion() )
		{
			if ( myType == ItemType.OPEN )
			{
				AudioPlayer.Play(openSound, m_audioSource);
				SetType( ItemType.CLOSE );
			}
			else if ( myType == ItemType.CLOSE )
			{
				AudioPlayer.Play(closeSound, m_audioSource);
				SetType( ItemType.OPEN );
			}
		}
	}

	public void TriggerMotion()
	{
		it.TriggerMotion();
	}
}
