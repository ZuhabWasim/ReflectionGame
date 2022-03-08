using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : InteractableAbstract
{
	private static int _id;
	private int m_id;
	public Texture img;
	protected override void OnStart()
	{
		myType = ItemType.PICKUP;
		m_id = _id++;
	}

	protected override void OnUserInteract()
	{
		this.gameObject.SetActive( false );
	}

	public void OnDrop( Vector3 dropPostion )
	{
		this.gameObject.SetActive( true );
		this.gameObject.transform.position = dropPostion;
	}
}
