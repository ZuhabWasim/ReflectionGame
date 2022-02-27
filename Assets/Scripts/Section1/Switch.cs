using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : InteractableAbstract
{

    public int switch_index;

    private GameObject fb;

    void Start()
    {
        fb = transform.parent.gameObject;
    }

    public override void OnUserInteract()
    {
        fb.GetComponent<Fusebox>().switchLight(switch_index);
    }
}
