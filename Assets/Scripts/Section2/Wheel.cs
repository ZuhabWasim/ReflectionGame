using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wheel : PickupItem
{
    public GameObject dressForm;
    public AudioClip formClipWheelFound;
    public AudioClip wheelClipFormFound;
    
    void Start()
    {
        base.OnStart();
        EventManager.Sub(Globals.Events.NEED_WHEEL, UpdateWheelClip);
    }

    void UpdateWheelClip()
    {
        voiceLine = wheelClipFormFound;
    }

    protected override void OnUserInteract()
    {
        InterpolateInteractableWrapper dressFormIIW = dressForm.GetComponent<InterpolateInteractableWrapper>();
        dressFormIIW.voiceLine = formClipWheelFound;
        dressFormIIW.nonUseableVoiceLine = formClipWheelFound;
    }
}
