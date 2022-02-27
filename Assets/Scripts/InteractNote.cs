using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{
    public AudioClip voiceline;

    public override void OnUserInteract()
    {   
         AudioPlayer.Play( Globals.AudioFiles.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
         AudioPlayer.Play( voiceline, Globals.Tags.DIALOGUE_SOURCE );
    }

}
