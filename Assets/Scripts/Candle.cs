using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script for Candle objects that can be used to tell the player where to go.
 *  Candles can be chained so that once a player visits a certain area, the next candle will light up.
 */
public class Candle : ProximityTrigger
{
    public Candle nextLight;
    private Light candleLight;
    
    protected override void OnStart()
    {
        candleLight = GetComponentInChildren<Light>();
        if (!active)
        {
            candleLight.intensity = 0f;
        }
    }
    protected override void OnActive()
    {
        candleLight.intensity = 40000f;
    }
    
    protected override void OnDeactive()
    {
        candleLight.intensity = 0f;
    }

    protected override void OnActivateTrigger()
    {
        if (nextLight != null)
        {
            nextLight.OnActive();
        }
        this.gameObject.SetActive(false);
    }
}
