using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{

    protected override void OnUserInteract()
    {   
         AudioPlayer.Play( Globals.AudioFiles.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
        //TODO show note text
    }

}
