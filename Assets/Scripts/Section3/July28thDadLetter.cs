using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class July28thDadLetter : InteractNote
{
    protected override void OnUserInteract()
    {
        base.OnUserInteract();
        AudioPlayer.Play(Globals.VoiceLines.Section3.JULY_28_LETTER, Globals.Tags.DIALOGUE_SOURCE);
        AudioPlayer.Play( Globals.VoiceLines.Section3.JULY_28_POV, Globals.Tags.DIALOGUE_SOURCE, false);
    }
}
