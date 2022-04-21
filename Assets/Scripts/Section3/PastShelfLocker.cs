using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastShelfLocker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Sub(Globals.Events.LOCK_PAST_DAD_SHELF, changeInterpDuration);
    }
    
    void changeInterpDuration()
    {
        GetComponent<InterpolateTransform>().interpDuration = 0f;
    }
}
