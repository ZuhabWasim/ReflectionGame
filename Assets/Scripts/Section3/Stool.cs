using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stool : InteractableAbstract
{
    protected override void OnUserInteract()
    {
        if ( EmptyBucket.IsOnBucket() )
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.STOOL_TOO_HIGH, Globals.Tags.DIALOGUE_SOURCE);
        }
        else
        {
            AudioPlayer.Play(Globals.VoiceLines.Section3.CANT_REACH, Globals.Tags.DIALOGUE_SOURCE);
        }
    }
}
