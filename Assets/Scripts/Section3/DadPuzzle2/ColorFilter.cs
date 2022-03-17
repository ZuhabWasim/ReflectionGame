using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFilter : MonoBehaviour
{
    public Light outgoingLight;
    public Color color;
    [Range(0.0f, 1.0f)]
    public float subtractiveFactor = 0.9f;
    
    public Color FilterColor( Color input )
    {
        EnableLight();
        outgoingLight.color = input - ( subtractiveFactor * color );
        return outgoingLight.color;
    }

    public void DisableLight()
    {
        outgoingLight.enabled = false;
    }

    public void EnableLight()
    {
        outgoingLight.enabled = true;
    }
}
