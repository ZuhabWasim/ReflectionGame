using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnEvent : MonoBehaviour
{
    [Tooltip("Event which should set this object to become inactive")]
    public string makeNonInteractableEvent = "";

    // Start is called before the first frame update
    void Start()
    {
        if (makeNonInteractableEvent != string.Empty)
        {
            EventManager.Sub(makeNonInteractableEvent, () => { Deactivate(); });
        }
    }

    void Deactivate()
    {
        this.gameObject.SetActive( false );
    }
}
