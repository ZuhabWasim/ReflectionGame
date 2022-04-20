using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZoneStateData
{
	public bool isPlayerInside;
	public float entryTime;
	public Zone zone;
}

public class Zone : MonoBehaviour
{
	[Tooltip( "If provided, this clip will be used to simulate footstep noise within this zone" )]
	public AudioClip footstepSound;
	private BoxCollider m_collider;
	private bool m_isPlayerInside = false;
	private float m_entryTime = -1f;
	void Start()
	{
		m_collider = GetComponent<BoxCollider>();
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.tag == Globals.Tags.PLAYER )
		{
			m_isPlayerInside = true;
			m_entryTime = Time.realtimeSinceStartup;
		}
	}

	void OnTriggerExit( Collider other )
	{
		if ( other.tag == Globals.Tags.PLAYER )
		{
			m_isPlayerInside = false;
			m_entryTime = -1f;
		}
	}

	public bool IsPlayerInside()
	{
		return m_isPlayerInside;
	}

	public ZoneStateData GetZoneState()
	{
		return new ZoneStateData() { isPlayerInside = m_isPlayerInside, entryTime = m_entryTime, zone = this };
	}
}
