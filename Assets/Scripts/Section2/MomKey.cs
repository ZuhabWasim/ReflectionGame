using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomKey : PickupItem
{
    void Start()
    {
        base.OnStart();    
    }

    protected override void OnUserInteract()
    {
        this.gameObject.SetActive( false );
        EventManager.Fire(Globals.Events.HAS_MOM_KEY);
    }
}
