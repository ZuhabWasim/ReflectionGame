using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        EventManager.Sub( Globals.Events.BLOCKING_MIRROR_B, OnBlockingMirrorB );
        EventManager.Sub( Globals.Events.UNBLOCKING_MIRROR_B, OnUnblockingMirrorB );
        
        moved = false;
        teleportable = false;
    }

    protected override void OnUserInteract()
    {
        if (moved)
        {
            
            // If the box is still blocking Mirror B, don't let the player teleport just yet.
            if ( isBoxBlockingMirror() )
            {
                AudioPlayer.Play( Globals.VoiceLines.Section2.CAN_T_GO_THROUGH_THIS_WAY, Globals.Tags.DIALOGUE_SOURCE );
                AudioPlayer.Play( Globals.VoiceLines.Section2.CAN_I_MOVE_THIS, Globals.Tags.DIALOGUE_SOURCE , false);
            }
            else
            {
                AudioPlayer.Play( Globals.VoiceLines.Section2.I_CAN_MOVE_THROUGH_IT_NOW, Globals.Tags.DIALOGUE_SOURCE );
                teleportable = true;
            }
        }
        else
        {
            InterpolateTransform pastMover = gameObject.GetComponentInParent<InterpolateTransform>();
            InterpolateTransform presMover = GameObject.Find("MirrorBPresent").GetComponent<InterpolateTransform>();
            
            pastMover.TriggerMotion();
            presMover.TriggerMotion();
            
            AudioPlayer.Play( Globals.VoiceLines.Section2.IS_IT_MOVING_IN_PRESENT, Globals.Tags.DIALOGUE_SOURCE);
            
            EventManager.Fire(Globals.Events.MOVE_MIRROR_B);
            
            teleportable = !isBoxBlockingMirror();
            moved = true;
        }
    }

    bool isBoxBlockingMirror()
    {
        GameObject scissorPuzzle = GameObject.Find(Globals.Misc.SCISSOR_PUZZLE);
        BlockMovingArea bma = scissorPuzzle.transform.Find("TileSpaces").GetComponent<BlockMovingArea>();

        return bma.BoxAtPosition(0, 0);

    }
    void OnSweepingDebris()
    {
        interactable = true;
    }
    
    
    void OnBlockingMirrorB()
    {
        if (moved)
        {
            teleportable = false;
        }
    }
    
    void OnUnblockingMirrorB()
    {
        if (moved)
        {
            teleportable = true;
        }
    }
}
