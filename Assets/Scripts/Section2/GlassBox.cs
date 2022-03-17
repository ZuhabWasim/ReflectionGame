using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBox : InterpolateInteractableWrapper
{
	
	private const float ALLOWABLE_BOX_HEIGHT = 1.1f;
	private const float ALLOWABLE_DISTANCE = 3.14f; // This requires the player to hug the shelf a bit.
	private const float ALLOWABLE_ERROR = 0.01f;
	
	
	/*
	protected override void OnStart()
	{
	}
	*/

	protected override void OnUserInteract()
	{
		if ( IsHighEnough() )
		{
			AudioPlayer.Play( Globals.VoiceLines.Section1.I_M_TOO_SMALL, Globals.Tags.DIALOGUE_SOURCE );
			TriggerMotion();
			interactable = false;
		}
	}

	private bool IsHighEnough()
	{
		Vector3 playerPosition = GameObject.FindGameObjectWithTag(Globals.Tags.PLAYER).transform.position;
		float distance = Vector3.Distance(playerPosition, this.transform.position);
		
		
		Debug.Log("playerPosition: " + playerPosition + "           Distance: " + Vector3.Distance(playerPosition, this.transform.position));
		
		// Millie's not high enough to reach the box.
		if ( playerPosition.y < ALLOWABLE_BOX_HEIGHT - ALLOWABLE_ERROR )
		{
			AudioPlayer.Play( voiceLine, Globals.Tags.DIALOGUE_SOURCE );
			return false;
		}
		
		// Millie is high enough but needs to get a bit closer.
		if ( distance > ALLOWABLE_DISTANCE + ALLOWABLE_ERROR )
		{
			AudioPlayer.Play( Globals.VoiceLines.Section1.DOOR_NOT_BUDGING, Globals.Tags.DIALOGUE_SOURCE );
			return false;
		}

		return true;
		// Millie is high enough and close enough.
		
		// Check if the player is close to the box and on at least the height of box 3.
		return playerPosition.y >= ALLOWABLE_BOX_HEIGHT - ALLOWABLE_ERROR &&
		       distance <= ALLOWABLE_DISTANCE + ALLOWABLE_ERROR;
		return false;
	}
}
