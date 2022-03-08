using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
	[Range( 0.1f, 1000f )]
	public float triggerDistance = Globals.Misc.DEFAULT_PROXIMITY_TRIGGER_DIST;
	public bool triggerOnce = true;
	[Tooltip( "Play this clip when this proximity trigger is activated" )]
	public AudioClip onTriggerClip;
	public string onTriggerClipSource = string.Empty;
	[Tooltip( "Fire this event when this proximity trigger is activated" )]
	public string onTriggerFireEvent = string.Empty;

	private GameObject m_player;
	private float sqrdTriggerDist;

	private const float TRIGGER_PLAYER_DIST_CHECK_TIME = 0.5f; // in seconds
	private const float TRIGGER_RESET_DELAY = 5f;
	void Start()
	{
		m_player = GameObject.FindGameObjectWithTag( Globals.Tags.PLAYER );
		sqrdTriggerDist = Mathf.Pow( triggerDistance, 2 );
		StartCoroutine( MonitorTriggerPlayerDist() );
	}

	float GetSqrdDistToPlayer()
	{
		return Vector3.SqrMagnitude( m_player.transform.position - this.gameObject.transform.position );
	}

	IEnumerator MonitorTriggerPlayerDist()
	{
		while ( true )
		{
			if ( GetSqrdDistToPlayer() <= sqrdTriggerDist )
			{
				ActivateTrigger();
				break;
			}

			yield return new WaitForSecondsRealtime( TRIGGER_PLAYER_DIST_CHECK_TIME );
		}
	}

	IEnumerator ResetTrigger()
	{
		// reset trigger after player leaves the trigger zone
		yield return new WaitUntil( () => { return GetSqrdDistToPlayer() > sqrdTriggerDist; } );
		yield return new WaitForSecondsRealtime( TRIGGER_RESET_DELAY );

		StartCoroutine( MonitorTriggerPlayerDist() );
	}

	void ActivateTrigger()
	{
		if ( onTriggerClip != null && onTriggerClipSource != string.Empty )
		{
			AudioPlayer.Play( onTriggerClip, onTriggerClipSource );
		}

		if ( onTriggerFireEvent != string.Empty )
		{
			EventManager.Fire( onTriggerFireEvent );
		}

		if ( !triggerOnce )
		{
			StartCoroutine( ResetTrigger() );
		}
	}
}
