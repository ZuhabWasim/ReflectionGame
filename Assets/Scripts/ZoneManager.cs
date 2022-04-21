using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{

	private Zone[] zones;
	private Zone m_activeZone;
	private const float ACTIVE_ZONE_REFRESH_RATE = 3;

	void Start()
	{
		zones = GameObject.FindObjectsOfType<Zone>();
		StartCoroutine( UpdateActiveZone() );
	}

	private IEnumerator UpdateActiveZone()
	{
		const float updateInterval = 1 / ACTIVE_ZONE_REFRESH_RATE;
		while ( true )
		{
			m_activeZone = GetCurrentPlayerZone();
			yield return new WaitForSecondsRealtime( updateInterval );
		}
	}

	public Zone GetCurrentPlayerZone()
	{
		foreach ( Zone zone in zones )
		{
			if ( zone.IsPlayerInside() )
			{
				return zone;
			}
		}

		return null;
	}

	public AudioClip GetCurrentFootstepSound()
	{
		if ( m_activeZone is null || m_activeZone.footstepSound is null )
		{
			return null;
		}

		return m_activeZone.footstepSound;
	}
}
