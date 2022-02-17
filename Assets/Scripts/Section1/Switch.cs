using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    public int switch_index;

    private GameObject fb;

    void Start()
    {
        fb = transform.parent.gameObject;

        //m_dialogueSource = GameObject.FindGameObjectWithTag(Globals.Tags.DIALOGUE_SOURCE).GetComponent<AudioSource>();
        EventManager.Sub(Globals.Events.INTERACT_KEY_PRESSED, OnUserInteract);
    }

    void OnUserInteract(GameObject player)
    {
        Transform camera = player.GetComponent<PlayerController>().playerCamera;
        RaycastHit hit;
        // Check if user is interacting with us
        if (Physics.Raycast(camera.position, camera.forward, out hit, Globals.Misc.MAX_INTERACT_DISTANCE)
            && hit.collider.gameObject.Equals(this.gameObject))
        {
            fb.GetComponent<Fusebox>().switchLight(switch_index);
        }
    }
}
