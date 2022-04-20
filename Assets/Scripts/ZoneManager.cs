using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{

	private Zone[] zones;
	private Zone m_activeZone;
	private List<ZoneStateData> m_activeZoneStates = new List<ZoneStateData>();
	private const float ACTIVE_ZONE_REFRESH_RATE = 3;

	void Start()
	{
		zones = GameObject.FindObjectsOfType<Zone>();
		Debug.LogFormat( "Collected {0} zones", zones.Length );
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
		m_activeZoneStates.Clear();
		foreach ( Zone zone in zones )
		{
			if ( zone.IsPlayerInside() )
			{
				m_activeZoneStates.Add( zone.GetZoneState() );
			}
		}

		if ( m_activeZoneStates.Count == 0 )
		{
			return null;
		}

		// get the last entered zone
		m_activeZoneStates.Sort(
			( ZoneStateData a, ZoneStateData b ) =>
			{
				return b.entryTime.CompareTo( a.entryTime );
			}
		);

		return m_activeZoneStates[ 0 ].zone;
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
