using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudioLight : InteractableAbstract
{
    /*COMPILERROR*/
    public Light spotLight;
    public AudioClip spotLightOnVoiceLine;
    
    // Start is called before the first frame update
    protected override void OnStart()
    {
        desiredItem = "Light Bulb";
    }
    
    protected override void OnUseItem()
    {
        EventManager.Fire( Globals.Events.DAD_PUZZLE_2_SPOTLIGHT_INSTALLED );
        voiceLine = spotLightOnVoiceLine;
        nonUseableVoiceLine = null;
    }
    
}
