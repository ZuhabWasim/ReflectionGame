using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawerPassageD : InteractableAbstract
{
    public GameObject drawerOne;
    public GameObject drawerTwo;
    public GameObject rampObject;
    
    protected override void OnStart()
    {
        desiredItem = Globals.UIStrings.DRAWER_PULL_ITEM;
    }

    protected override void OnUseItem()
    {
        drawerOne.GetComponent<InterpolateTransform>().TriggerMotion();
        drawerTwo.GetComponent<InterpolateTransform>().TriggerMotion();
        rampObject.SetActive(true);
    }
}
