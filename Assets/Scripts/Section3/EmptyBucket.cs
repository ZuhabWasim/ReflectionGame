using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBucket : PickupItem
{
	private static float FLOOR_HEIGHT = 0.27f;
	private static float BUCKET_HEIGHT = 1.25f;
	private static float HEIGHT_ERROR = 0.02f;
	
	public override void OnUserInteractUnfiltered()
	{
		base.OnUserInteract();
	}

	private void SetDropTransform()
	{
		this.gameObject.transform.eulerAngles = new Vector3( 90, 0, 0 );
	}

	public override void OnDrop( Vector3 dropPostion, bool isLocal = false )
	{
		base.OnDrop( dropPostion, isLocal );
		SetDropTransform();
		Inventory inventory = Inventory.GetInstance();
		PickupItem selectedItem = inventory.GetSelectedPickupItem();
		inventory.DeleteItem( selectedItem.itemName );
	}
	
	public static bool IsOnBucket()
	{
		Vector3 playerPosition = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).transform.position;
		return playerPosition.y >= BUCKET_HEIGHT - HEIGHT_ERROR;
	}
}
