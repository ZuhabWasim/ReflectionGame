using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHandkerchief : InteractableAbstract
{
    private const string MIRROR_AUDIO_SOURCE = "MirrorAudioSource";
    public AudioClip soundEffect;
    public MirrorPlane dirtyMirror; // TODO(dennis): remove this
    private Inventory m_inventory;
    
    protected override void OnStart()
    {
        m_inventory = Inventory.GetInstance();
        AudioPlayer.RegisterAudioPlayer(MIRROR_AUDIO_SOURCE, GetComponent<AudioSource>());
        
        desiredItem = Globals.UIStrings.HANDKERCHIEF_ITEM;
    }

    protected override void OnUserInteract()
    {   
        AudioPlayer.Play( Globals.AudioFiles.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
    }

    protected override void OnUseItem()
    {
        Debug.Log("Using handkerchief");
        m_inventory.DeleteItem(desiredItem);
        HandleInteract();
    }

    void HandleInteract()
    {
        // We can probably keep the above function as inheritable and do all specific things like changing the dirty texture here
        AudioPlayer.Play( soundEffect, MIRROR_AUDIO_SOURCE );

        if (dirtyMirror != null)
        {
            dirtyMirror.CleanMirror();
        }
    }
}
