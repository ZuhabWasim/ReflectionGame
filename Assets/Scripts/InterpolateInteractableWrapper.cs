using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateInteractableWrapper : InteractableAbstract
{
    private InterpolateTransform it;
    
    // Start is called before the first frame update
    void Start()
    {
        it = GetComponent<InterpolateTransform>();
    }

    protected override void OnUserInteract()
    {
        if (it.ActivateInteractMotion())
        {
            if (myType == ItemType.OPEN)
            {
                SetType(ItemType.CLOSE);
            }
            else if (myType == ItemType.CLOSE)
            {
                SetType(ItemType.OPEN);
            }
        }
    }
}
