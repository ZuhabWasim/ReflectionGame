using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBucket : InteractableAbstract
{
    
    protected override void OnStart()
    {
        desiredItem = "Paint Brush";
    }

	protected override void OnUseItem()
	{
		EventManager.Fire( Globals.Events.PAINT_BRUSH_WET );
	}
}
