using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PaintType
{
	NONE = 0,
	WHITE,
	REFLECTIVE,
	PORTAL
}

public class PaintBucket : InteractableAbstract
{
	public PaintType paint = PaintType.NONE;
	public Texture icon;
	
	protected override void OnStart()
	{
		desiredItem = Globals.Misc.PAINT_BRUSH;
	}

	protected override void OnUseItem()
	{
		Inventory inventory = Inventory.GetInstance();
		PickupItem item = inventory.GetSelectedPickupItem();
		PaintBrush brush = item.GetComponent<PaintBrush>();
		brush.paint = paint;
		if (paint == PaintType.WHITE) {
			brush.itemName = Globals.Misc.WHITE_PAINT_BRUSH;
		} else
        {
			brush.itemName = Globals.Misc.WET_PAINT_BRUSH;
		}
		brush.img = icon;
		inventory.RefreshItem( item );
	}
}
