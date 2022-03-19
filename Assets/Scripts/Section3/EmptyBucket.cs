using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBucket : PickupItem
{
	// Start is called before the first frame update
	protected override void OnStart()
	{
	}

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
		inventory.DeleteItem( this.itemName );
	}
}
