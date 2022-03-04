using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : InteractableAbstract
{

    public int switch_index;

    private GameObject fb;

    protected override void OnStart()
    {
        fb = transform.parent.gameObject;
    }

    protected override void OnUserInteract()
    {
        fb.GetComponent<Fusebox>().switchLight(switch_index);
    }
}
