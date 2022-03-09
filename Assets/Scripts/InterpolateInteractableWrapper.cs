using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateInteractableWrapper : InteractableAbstract
{
	//THIS OBJECT MUST ALSO HAVE AN INTERPOLATETRANSFORM SCRIPT

	private InterpolateTransform it;

	protected override void OnStart()
	{
		it = GetComponent<InterpolateTransform>();
		EventManager.Sub( Globals.Events.LOCK_MOM_DOOR, () => { LockDoor(); } );
	}

	protected override void OnUserInteract()
	{
		if ( it.ActivateInteractMotion() )
		{
			if ( myType == ItemType.OPEN )
			{
				SetType( ItemType.CLOSE );
			}
			else if ( myType == ItemType.CLOSE )
			{
				SetType( ItemType.OPEN );
			}
		}
	}

	void LockDoor()
	{
		Debug.Log("LockMomDoor");
		SetType( ItemType.CLOSE );
		it.TriggerMotion();
		interactable = false;
		
	}
}
