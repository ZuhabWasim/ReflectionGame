using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomNote : InteractNote
{
    protected override void OnUserInteract()
    {
        base.OnUserInteract();
        AudioPlayer.Play(Globals.VoiceLines.Section2.MOM_NOTE, Globals.Tags.DIALOGUE_SOURCE);
        EventManager.Fire(onReadFireEvent);
    }
}