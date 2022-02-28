using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : MonoBehaviour
{
    public AudioClip voiceline;
    private GameObject m_player;
    void Start()
    {
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
            AudioPlayer.Play( Globals.AudioFiles.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
            HandleInteract();
        }
    }

    void HandleInteract()
    {
        AudioPlayer.Play( voiceline, Globals.Tags.DIALOGUE_SOURCE );
    }
}
