using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorB : MirrorInteractable
{
    private bool moved;
    
    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
        
        EventManager.Sub( Globals.Events.SWEPT_DEBRIS, OnSweepingDebris );
        
        moved = false;
        teleportable = false;
    }

    protected override void OnUserInteract()
    {
        if (moved)
        {
            AudioPlayer.Play( Globals.VoiceLines.Section2.NO_MORE_OBSTRUCTIONS, Globals.Tags.DIALOGUE_SOURCE );
        }
        else
        {
            InterpolateTransform pastMover = gameObject.GetComponentInParent<InterpolateTransform>();
            InterpolateTransform presMover = GameObject.Find("MirrorBPresent").GetComponent<InterpolateTransform>();
            
            pastMover.TriggerMotion();
            presMover.TriggerMotion();
            
            AudioPlayer.Play( Globals.AudioFiles.Section2.MOVING_MIRROR, Globals.Tags.MAIN_SOURCE); // Not working so use MainSource
            
            EventManager.Fire(Globals.Events.MOVE_MIRROR_B);
            
            teleportable = true;
            moved = true;
        }
    }

    void OnSweepingDebris()
    {
        interactable = true;
    }

}
