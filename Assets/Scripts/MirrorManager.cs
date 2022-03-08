using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Any state mirror management should happen here.
public class MirrorManager : MonoBehaviour
{
	public MirrorConnector section1Mirror;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		bool canTP = false;

		canTP |= section1Mirror.CanTeleport();

		GlobalState.SetVar<bool>( Globals.Vars.CAN_TELEPORT, canTP );
	}
}
