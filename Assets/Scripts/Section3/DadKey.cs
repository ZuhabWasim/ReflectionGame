using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadKey : PickupItem
{
    void Start()
    {
        base.OnStart();    
    }

    protected override void OnUserInteract()
    {
        this.gameObject.SetActive( false );
        EventManager.Fire(Globals.Events.HAS_DAD_KEY);
        UpdateMover();
    }
    
    private void UpdateMover()
    {
        InterpolateTransform it = GameObject.FindWithTag(Globals.Tags.PRESENT_DAD_SHELF).GetComponent<InterpolateTransform>();
        it.startRotation = new Vector3(0, 90, 0);  // To fully open up the closet.
        it.interpDuration = 2f;
    }
}
