using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MomKey : PickupItem
{
    public List<GameObject> mirrorsToDeactivate;
    void Start()
    {
        base.OnStart();    
    }

    protected override void OnUserInteract()
    {
        this.gameObject.SetActive( false );
        EventManager.Fire(Globals.Events.HAS_MOM_KEY);
        foreach (GameObject mirrorObject in mirrorsToDeactivate)
        {
            MirrorInteractable mirror = mirrorObject.GetComponent<MirrorInteractable>();
            mirror.nonTeleportableVoiceLine = nonInteractableVoiceLine;
        }
    }
}
