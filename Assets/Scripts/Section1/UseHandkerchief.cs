using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHandkerchief : MonoBehaviour
{
    private const string MIRROR_AUDIO_SOURCE = "MirrorAudioSource";
    public string itemName;
    public AudioClip soundEffect;
    public MirrorPlane dirtyMirror; // TODO(dennis): remove this
    private Inventory m_inventory;
    private GameObject m_player;
    
    void Start()
    {
        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INTERACT_KEY ), OnUserInteract );
        m_inventory = Inventory.GetInstance();
        m_player = GameObject.FindGameObjectWithTag( Globals.Tags.PLAYER );
        AudioPlayer.RegisterAudioPlayer( MIRROR_AUDIO_SOURCE, GetComponent<AudioSource>() );
    }

    void OnUserInteract()
    {   
        Transform camera = m_player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        if ( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
             && hit.collider.gameObject.Equals( this.gameObject ))
        {
            if (m_inventory.GetSelectedItem().Equals(itemName))
            {
                Debug.Log( "Using handkerchief" );
                m_inventory.DeleteItem(itemName);
                HandleInteract();
            }
            else
            {
                AudioPlayer.Play( Globals.AudioFiles.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
            }
            
        }
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
