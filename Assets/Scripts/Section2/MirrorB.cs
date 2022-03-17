using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorB : MirrorInteractable
{
    private bool moved;
    private bool sweptDebris;
    
    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
        
        moved = false;
        sweptDebris = false;
        
        EventManager.Sub( Globals.Events.SWEPT_DEBRIS, OnSweepingDebris );
    }

    protected override void OnUserInteract()
    {
        if (moved)
        {
            AudioPlayer.Play( Globals.VoiceLines.Section1.I_M_TOO_SMALL, Globals.Tags.DIALOGUE_SOURCE );
        }
        else
        {
            InterpolateTransform it = gameObject.GetComponentInParent<InterpolateTransform>();
            it.TriggerMotion();
            teleportable = true;
            moved = true;
        }
    }
    
    void OnSweepingDebris()
    {
        sweptDebris = true;
        interactable = true;
    }

}
