using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class EmptyBucket : PickupItem
{
	public static float FLOOR_HEIGHT = 0.27f;
	public static float BUCKET_HEIGHT = 1.25f;
	public static float HEIGHT_ERROR = 0.02f;
	private bool repeated = false;
	
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
		SetDropTransform();
		Inventory inventory = Inventory.GetInstance();
		PickupItem selectedItem = inventory.GetSelectedPickupItem();
		inventory.DeleteItem( selectedItem.itemName );
		
		// Spawn the bucket right below the player, prop the player on top of the bucket, if they fall, they can climb back up.
		GameObject player = GameObject.FindGameObjectWithTag( Globals.Tags.PLAYER );
		Vector3 floorHeight = (GlobalState.GetVar<bool>("isPresent")) ? Vector3.zero : new Vector3(0, FLOOR_HEIGHT, 0);

		// Note that BUCKET_HEIGHT includes the past floor height
		Vector3 playerCurrentPos = player.transform.position;
		Vector3 newDropPosition = new Vector3(playerCurrentPos.x, 
			playerCurrentPos.y + EmptyBucket.BUCKET_HEIGHT / 2 + HEIGHT_ERROR, playerCurrentPos.z);
		player.transform.position += new Vector3(0, EmptyBucket.BUCKET_HEIGHT + HEIGHT_ERROR + 0.1f, 0);
		// player.transform.position = player.transform.position + new Vector3(0, EmptyBucket.BUCKET_HEIGHT + 0.2f, 0);
		// Vector3 newDropPosition = new Vector3(player.transform.position.x, EmptyBucket.BUCKET_HEIGHT / 2 + 0.15f, player.transform.position.z);
		
		// Drop the item with this specific drop position.
		base.OnDrop( newDropPosition, isLocal );
		this.GetComponent<Rigidbody>().useGravity = true;
		
		// Stop playing the line if the player already heard it twice
		if (repeated)
		{
			voiceLine = null;
		}
		else
		{
			repeated = true;
		}
	}
	
	public static bool IsOnBucket()
	{
		Vector3 playerPosition = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).transform.position;
		return playerPosition.y >= BUCKET_HEIGHT - HEIGHT_ERROR;
	}
}
