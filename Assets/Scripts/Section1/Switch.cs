using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    public int switch_index;

    private GameObject fb;
    private GameObject m_player;

    void Start()
    {
        fb = transform.parent.gameObject;
        m_player = GameObject.FindGameObjectWithTag( Globals.Tags.PLAYER );
        EventManager.Sub( InputManager.GetKeyDownEventName( Keybinds.INTERACT_KEY ), OnUserInteract );
    }

    void OnUserInteract()
    {
        Transform camera = m_player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        // Check if user is interacting with us
        if (Physics.Raycast(camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE)
            && hit.collider.gameObject.Equals(this.gameObject))
        {
            fb.GetComponent<Fusebox>().switchLight(switch_index);
        }
    }
}
