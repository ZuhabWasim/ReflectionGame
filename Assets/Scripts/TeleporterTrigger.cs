using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTrigger : MonoBehaviour
{
	private Teleporter m_teleporter;

	public void SetTeleporter( Teleporter teleporter )
	{
		m_teleporter = teleporter;
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.tag == Globals.Tags.PLAYER )
		{
			Debug.Log( "Can Teleport now" );
			m_teleporter.SetCanTeleport( true );
		}
	}

	void OnTriggerExit( Collider other )
	{
		if ( other.tag == Globals.Tags.PLAYER )
		{
			Debug.Log( "Left Teleport zone" );
			m_teleporter.SetCanTeleport( false );
		}
	}
}
