using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHandkerchief : MonoBehaviour
{
    public string itemName;
    public AudioClip soundEffect;
    public MirrorPlane dirtyMirror; // TODO(dennis): remove this


    private AudioSource m_soundSource;
    private Inventory m_inventory;
    
    void Start()
    {
        EventManager.Sub( Globals.Events.INTERACT_KEY_PRESSED, OnUserInteract );
        m_inventory = Inventory.GetInstance();
        m_soundSource = GetComponent<AudioSource>();
    }

    void OnUserInteract( GameObject player )
    {   
        Transform camera = player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        if ( Physics.Raycast( camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE )
             && hit.collider.gameObject.Equals( this.gameObject ) 
             && m_inventory.GetSelectedItem().Equals(itemName))
        {
            Debug.Log( "Using handkerchief" );
            m_inventory.DeleteItem(itemName);
            HandleInteract();
        }
    }

    void HandleInteract()
    {
        // We can probably keep the above function as inheritable and do all specific things like changing the dirty texture here
        m_soundSource.clip = soundEffect;
        m_soundSource.Play();

        if (dirtyMirror != null)
        {
            dirtyMirror.CleanMirror();
        }
    }
}
