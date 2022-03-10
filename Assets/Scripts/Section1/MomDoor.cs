using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomDoor : InterpolateInteractableWrapper
{
    // Start is called before the first frame update
    new void Start()
    {
        EventManager.Sub( Globals.Events.LOCK_MOM_DOOR, () => { LockDoor(); } );
        Debug.Log("i am here too");
    }

    void LockDoor()
    {
        Debug.Log("mom lock door xd");
        SetType( ItemType.CLOSE );
        TriggerMotion();
        interactable = false;
    }
}
