using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
	[Tooltip( "If provided, this clip will be used to simulate footstep noise within this zone" )]
	public AudioClip footstepSound;
	private BoxCollider m_collider;
	private bool m_isPlayerInside = false;
	void Start()
	{
		m_collider = GetComponent<BoxCollider>();
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.tag == Globals.Tags.PLAYER )
		{
			m_isPlayerInside = true;
		}
	}

	void OnTriggerExit( Collider other )
	{
		if ( other.tag == Globals.Tags.PLAYER )
		{
			m_isPlayerInside = false;
		}
	}

	public bool IsPlayerInside()
	{
		return m_isPlayerInside;
	}
}
