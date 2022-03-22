using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBucket : PickupItem
{
	public override void OnUserInteractUnfiltered()
	{
		base.OnUserInteract();
	}

	private void SetDropTransform()
	{
		this.gameObject.transform.eulerAngles = new Vector3( 180, 0, 0 );
	}

	public override void OnDrop( Vector3 dropPostion, bool isLocal = false )
	{
		base.OnDrop( dropPostion, isLocal );
		SetDropTransform();
		Inventory inventory = Inventory.GetInstance();
		PickupItem selectedItem = inventory.GetSelectedPickupItem();
		inventory.DeleteItem( selectedItem.itemName );
	}
}
