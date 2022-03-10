using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomDoor : InterpolateInteractableWrapper
{
    // Start is called before the first frame update
    protected override void OnStart()
    {
        EventManager.Sub( Globals.Events.LOCK_MOM_DOOR, () => { LockDoor(); } );
    }

    void LockDoor()
    {
        SetType( ItemType.CLOSE );
        TriggerMotion();
        interactable = false;
    }
}
