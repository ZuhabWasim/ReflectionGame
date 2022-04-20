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
        EventManager.Sub(Globals.Events.USED_WHEEL, RemoveWheelClips);
    }

    void UpdateWheelClip()
    {
        voiceLine = wheelClipFormFound;
    }

    void RemoveWheelClips()
    {
        InterpolateInteractableWrapper dressFormIIW = dressForm.GetComponent<InterpolateInteractableWrapper>();
        dressFormIIW.voiceLine = null;
        dressFormIIW.nonUseableVoiceLine = null;
    }
    
    protected override void OnUserInteract()
    {
        InterpolateInteractableWrapper dressFormIIW = dressForm.GetComponent<InterpolateInteractableWrapper>();
        dressFormIIW.voiceLine = formClipWheelFound;
        dressFormIIW.nonUseableVoiceLine = formClipWheelFound;
    }
}
