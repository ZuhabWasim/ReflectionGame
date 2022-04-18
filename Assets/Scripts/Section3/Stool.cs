using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stool : InteractableAbstract
{
    protected override void OnUserInteract()
    {
        if ( EmptyBucket.IsOnBucket() )
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.STILL_TOO_HIGH_USE_BUCKET, Globals.Tags.DIALOGUE_SOURCE);
        }
        else
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.CANT_REACH_IRONIC, Globals.Tags.DIALOGUE_SOURCE);
        }
    }
}
