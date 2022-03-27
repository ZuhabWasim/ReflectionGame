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
        spotLight.range = 0f;
        spotLight.intensity = 0f;
    }
    
    protected override void OnUseItem()
    {
        spotLight.intensity = 80000;
        spotLight.range = 4.76f;
    }
    
}
