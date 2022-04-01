using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentShelf : InteractableAbstract
{
    private InterpolateTransform mover = null;
    
    // Start is called before the first frame update
    protected override void OnStart()
    {
        EventManager.Sub( Globals.Events.UPDATE_MOVEMENT, UpdateMover );
        EventManager.Sub( Globals.Events.DAD_PUZZLE_1_BOOKSHELF_SOLVED, OpenShelf );
        mover = GetComponent<InterpolateTransform>();
        desiredItem = Globals.UIStrings.CROWBAR_ITEM;
    }

    protected override void OnUseItem()
    {
        OpenShelf();
    }

    private void OpenShelf()
    {
        mover.TriggerMotion();
    }

    private void UpdateMover()
    {
        mover.startRotation = new Vector3( 0, 90, 0 );  // To fully open up the closet.
        mover.interpDuration = 2f;
    }
}
