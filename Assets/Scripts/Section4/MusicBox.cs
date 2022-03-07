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
    
    protected override void OnStart()
    {
        EventManager.Sub(Globals.Events.MILLIE_KEY_INTERACT, MillieKeyInteract);
        
        m_inventory = Inventory.GetInstance();
        // AudioPlayer.RegisterAudioPlayer(MUSICBOX_AUDIO_SOURCE, GetComponent<AudioSource>());
        
        desiredItem = Globals.UIStrings.MUSICBOXMILLIE_ITEM;
    }

    protected override void OnUserInteract()
    {   
        AudioPlayer.Play( voiceLine, Globals.Tags.MAIN_SOURCE );
    }

    protected override void OnUseItem()
    {
        Debug.Log("Initially interacting with the Music Box with Millie's key");
        HandleInteract();
    }

    void HandleInteract()
    {
        MillieKeyInteract();
    }

    void MillieKeyInteract()
    {
        Debug.Log("MillieKeyInteract");
        AudioPlayer.Play( millieKeyVoiceline, Globals.Tags.MAIN_SOURCE );
    }

}
