using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : InteractableAbstract
{
    private const string DEBRIS_AUDIO_SOURCE = "DebrisAudioSource";

    public AudioClip soundEffect;
    public AudioClip sweepingDialogue;
	
    protected override void OnStart()
    {
        AudioPlayer.RegisterAudioPlayer(DEBRIS_AUDIO_SOURCE, GetComponent<AudioSource>());

        desiredItem = Globals.UIStrings.BROOM_ITEM;
    }

    protected override void OnUseItem()
    {
        Debug.Log( "Sweeping Debris Away" );
        
        this.gameObject.SetActive(false);
        
        AudioPlayer.Play( soundEffect, DEBRIS_AUDIO_SOURCE );
        AudioPlayer.Play( sweepingDialogue, Globals.Tags.DIALOGUE_SOURCE );
        
        EventManager.Fire( Globals.Events.SWEPT_DEBRIS );
    }
}
