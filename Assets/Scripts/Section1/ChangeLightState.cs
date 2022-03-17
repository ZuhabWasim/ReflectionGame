using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightState : MonoBehaviour
{
	public GameObject[] lightSources;

	public GameObject[] whiteEmissiveTextures;
	public Material whiteEmissiveOn;
	public Material whiteEmissiveOff;

	public AudioClip soundEffect;
	private const string FUSEBOX_AUDIO_SOURCE = "FuseBoxAudioSource";
	
	// Start is called before the first frame update
	void Start()
	{
		EventManager.Sub( Globals.Events.LIGHTS_TURN_ON, TurnLightsOn );
		EventManager.Sub( Globals.Events.LIGHTS_TURN_OFF, TurnLightsOff );
		
		TurnLightsOff();
	}

	void TurnLightsOn()
	{
		for ( int i = 0; i < whiteEmissiveTextures.Length; i++ )
		{
			whiteEmissiveTextures[ i ].GetComponent<Renderer>().material = whiteEmissiveOn;
		}

		for ( int i = 0; i < lightSources.Length; i++ )
		{
			lightSources[ i ].SetActive( true );
		}
		AudioPlayer.Play(soundEffect, FUSEBOX_AUDIO_SOURCE, false);
		AudioPlayer.Play( Globals.VoiceLines.Section1.HOUSE_FEELS_UNFAMILIAR, Globals.Tags.DIALOGUE_SOURCE );
	}


	void TurnLightsOff()
	{
		for ( int i = 0; i < whiteEmissiveTextures.Length; i++ )
		{
			whiteEmissiveTextures[ i ].GetComponent<Renderer>().material = whiteEmissiveOff;
		}

		for ( int i = 0; i < lightSources.Length; i++ )
		{
			lightSources[ i ].SetActive( false );
		}
	}

}
