using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : MonoBehaviour
{
    public AudioClip voiceline;
    private AudioSource m_dialogueSource;
    private GameObject m_player;
    void Start()
    {
        m_dialogueSource = GameObject.FindGameObjectWithTag( Globals.Tags.DIALOGUE_SOURCE ).GetComponent<AudioSource>();
        m_player = GameObject.FindGameObjectWithTag( Globals.Tags.PLAYER );
        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INTERACT_KEY ), OnUserInteract );
    }

    void OnUserInteract()
    {   
        Transform camera = m_player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        // Check if user is interacting with us
        if ( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
            && hit.collider.gameObject.Equals( this.gameObject ) )
        {
            PlayerController.PlaySound( "paper_unravel" );
            HandleInteract();
        }
    }

    void HandleInteract()
    {
        if ( m_dialogueSource.isPlaying )
        {
            m_dialogueSource.Stop();
        }

        m_dialogueSource.clip = voiceline;
        m_dialogueSource.Play();
    }
}
