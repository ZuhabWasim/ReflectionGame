using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingRack : InteractableAbstract
{
	private const string CLOTHING_RACK_AUDIO_SOURCE = "ClothingRackAudioSource";
	
	public AudioClip soundEffect;
	public AudioClip cuttingDialogue;
	protected override void OnStart()
	{
		EventManager.Sub( Globals.Events.OBTAINED_SCISSORS, OnObtainingScissors );

		AudioPlayer.RegisterAudioPlayer(CLOTHING_RACK_AUDIO_SOURCE, GetComponent<AudioSource>());

		desiredItem = Globals.UIStrings.SCISSORS_ITEM;
	}

	protected override void OnUseItem()
	{
		HandleUseItem();
	}

	void HandleUseItem()
	{
	    
		Debug.Log( "desiredItem: " + desiredItem );
		Debug.Log( "Cutting Clothing Rack" );
        
		this.gameObject.SetActive(false);
        
		AudioPlayer.Play( soundEffect, CLOTHING_RACK_AUDIO_SOURCE );
		AudioPlayer.Play( cuttingDialogue, Globals.Tags.DIALOGUE_SOURCE );
        
		EventManager.Fire( Globals.Events.CUTTING_CLOTHING_RACK );
	}

	void OnObtainingScissors()
	{
		Debug.Log("Obtained Scissors");
	}
}
