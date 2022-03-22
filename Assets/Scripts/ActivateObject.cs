using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObject : MonoBehaviour
{
    public string activateEvent = "";
    public string deactivateEvent = "";

    public GameObject activeObject;

    public void Start()
    {
        if ( activateEvent != string.Empty )
        {
            EventManager.Sub( activateEvent, () => { Activate(); } );
        }

        if ( deactivateEvent != string.Empty )
        {
            EventManager.Sub( deactivateEvent, () => { Deactivate(); } );
        }
    }

    public void Activate()
    {
        activeObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        activeObject.SetActive(false);
    }
}
