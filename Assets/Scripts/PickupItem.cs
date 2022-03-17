using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : InteractableAbstract
{
	private static int _id;
	private int m_id;
	public Texture img;
	public string onPickupEvent;

	protected override void OnStart()
	{
		myType = ItemType.PICKUP;
		m_id = _id++;
	}

	protected override void OnUserInteract()
	{
		this.gameObject.SetActive( false );
		if ( onPickupEvent == string.Empty ) return;
		EventManager.Fire( onPickupEvent, this.gameObject );
	}

	public virtual void OnDrop( Vector3 dropPostion, bool isLocal = false )
	{
		this.gameObject.SetActive( true );
		if ( isLocal ) this.gameObject.transform.localPosition = dropPostion;
		else this.gameObject.transform.position = dropPostion;
	}
}
