using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHandkerchief : MonoBehaviour
{
    public string itemName;
    public AudioClip soundEffect;
    private AudioSource m_soundSource;
    public Inventory playerInventory;
    
    void Start()
    {
        EventManager.Sub( Globals.Events.INTERACT_KEY_PRESSED, OnUserInteract );
        playerInventory = Inventory.GetInstance();
        m_soundSource = GetComponent<AudioSource>();
    }

    void OnUserInteract( GameObject player )
    {   
        Transform camera = player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        // Check if user is interacting with us
        /*Debug.Log( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
                   + ",   " + hit.collider.gameObject.Equals( this.gameObject ) 
                   + ",   " + playerInventory.GetSelectedItem()
                   + ",    (" + itemName + ")");*/
        if ( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
             && hit.collider.gameObject.Equals( this.gameObject ) 
             && playerInventory.GetSelectedItem().Equals(itemName))
        {
            Debug.Log( "Using handkerchief" );
            playerInventory.DeleteItem(itemName);
            HandleInteract();
        }
    }

    void HandleInteract()
    {
        // We can probably keep the above function as inheritable and do all specific things like changing the dirty texture here
        m_soundSource.clip = soundEffect;
        m_soundSource.Play();
    }
}