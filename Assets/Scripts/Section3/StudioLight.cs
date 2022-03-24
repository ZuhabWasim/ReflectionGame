using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudioLight : InteractableAbstract
{
    /*COMPILERROR*/
    public Light spotLight;

    // Start is called before the first frame update
    void Start()
    {
        desiredItem = "Light Bulb";
        spotLight.intensity = 0;
    }
    
    protected override void OnUseItem()
    {
        spotLight.intensity = 0;
    }
    
}
