using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingRack : InteractableAbstract
{
	private const string CLOTHING_RACK_AUDIO_SOURCE = "ClothingRackAudioSource";
	public AudioClip soundEffect;
	public AudioClip cuttingDialogue;

	private string audioSource;
	
	protected override void OnStart()
	{
		EventManager.Sub( Globals.Events.OBTAINED_SCISSORS, OnObtainingScissors );

		audioSource = CLOTHING_RACK_AUDIO_SOURCE + this.name;
		AudioPlayer.RegisterAudioPlayer(audioSource, GetComponent<AudioSource>());

		desiredItem = Globals.UIStrings.SCISSORS_ITEM;
	}

	protected override void OnUseItem()
	{
		HandleUseItem();
	}

	void HandleUseItem()
	{
		Debug.Log( "Cutting Clothing Rack" );
        
		this.gameObject.SetActive(false);
        
		AudioPlayer.Play( soundEffect, Globals.Tags.MAIN_SOURCE ); // Use Main source for now until the models come in.
		AudioPlayer.Play( cuttingDialogue, Globals.Tags.DIALOGUE_SOURCE );
        
		EventManager.Fire( Globals.Events.CUTTING_CLOTHING_RACK );
	}

	void OnObtainingScissors()
	{
		Debug.Log("Obtained Scissors");
	}
}
