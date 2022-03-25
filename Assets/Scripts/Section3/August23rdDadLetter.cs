using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class August23rdDadLetter : InteractNote
{
    protected override void OnUserInteract()
    {
        base.OnUserInteract();
        AudioPlayer.Play(Globals.VoiceLines.Section3.AUG_23_LETTER, Globals.Tags.DIALOGUE_SOURCE);
        AudioPlayer.Play( Globals.VoiceLines.Section3.AUG_23_POV, Globals.Tags.DIALOGUE_SOURCE, false);
    }
}
