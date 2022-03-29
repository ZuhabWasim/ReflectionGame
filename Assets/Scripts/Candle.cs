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
            candleLight.enabled = false;
        }
    }
    protected override void OnActive()
    {
        candleLight.enabled = true;
    }
    
    protected override void OnDeactive()
    {
        candleLight.enabled = false;
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
