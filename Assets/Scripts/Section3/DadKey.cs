using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadKey : PickupItem
{
	
	protected override void OnUserInteract()
	{
		this.gameObject.SetActive( false );
		EventManager.Fire( Globals.Events.HAS_DAD_KEY );
		
		// Delete brushes from inventory. 
		Inventory inventory = Inventory.GetInstance();
		inventory.DeleteItem(Globals.Misc.WET_PAINT_BRUSH);
		inventory.DeleteItem(Globals.Misc.WHITE_PAINT_BRUSH);
	}
}
