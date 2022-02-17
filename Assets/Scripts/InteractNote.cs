using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : MonoBehaviour
{
    public AudioClip voiceline;
    private AudioSource m_dialogueSource;
    void Start()
    {
        m_dialogueSource = GameObject.FindGameObjectWithTag( Globals.Tags.DIALOGUE_SOURCE ).GetComponent<AudioSource>();
        EventManager.Sub( Globals.Events.INTERACT_KEY_PRESSED, OnUserInteract );
    }

    void OnUserInteract( GameObject player )
    {   
        Transform camera = player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        // Check if user is interacting with us
        if ( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
            && hit.collider.gameObject.Equals( this.gameObject ) )
        {
            Debug.Log( "Playing audio from script" );
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
