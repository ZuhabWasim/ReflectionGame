using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is functionality for (event_1 && ... && event_n) => event_n+1

public class Fork
{
	public int num_inputs;
	public string output_event;
	bool[] tracker;

	public Fork( string[] i, string o )
	{
		num_inputs = i.Length;
		output_event = o;
		tracker = new bool[ num_inputs ];
		for ( int j = 0; j < num_inputs; j++ )
		{
			tracker[ j ] = false;
			int n = j;
			EventManager.Sub( i[ j ], () => { UpdateTracker( n ); } );
		}
	}

	void UpdateTracker( int n )
	{
		tracker.SetValue( true, n );
		if ( ForkComplete() )
		{
			EventManager.Fire( output_event );
		}
		Debug.Log( tracker );
	}

	bool ForkComplete()
	{
		for ( int j = 0; j < num_inputs; j++ )
		{
			if ( !tracker[ j ] )
			{
				return false;
			}
		}
		return true;
	}

}

public static class ForkList
{
	static List<Fork> forks;

	public static void InitForks()
	{
		forks = new List<Fork>();

		// (GO_CLEAN_MIRROR && GO_ENTER_CODE) => DONE_INVESTIGATE
		forks.Add( new Fork( new string[] { Globals.Events.GO_CLEAN_MIRROR,
			Globals.Events.GO_ENTER_CODE },
			Globals.Events.DONE_INVESTIGATE ) );

		// (HAS_MOM_KEY && HAS_DAD_KEY) => PICKUP_BOTH_KEY
		forks.Add(new Fork(new string[] { Globals.Events.HAS_MOM_KEY,
			Globals.Events.HAS_DAD_KEY },
			Globals.Events.PICKUP_BOTH_KEY));

		// (EXIT_MOM && EXIT_DAD) => EXIT_BOTH
		forks.Add(new Fork(new string[] { Globals.Events.EXIT_DAD,
			Globals.Events.EXIT_MOM },
			Globals.Events.EXIT_BOTH));

		// (LOCK_MOM_DOOR || LOCK_DAD_DOOR) => LOCK_EITHER_DOOR
		EventManager.Sub(Globals.Events.LOCK_MOM_DOOR, () => { EventManager.Fire(Globals.Events.LOCK_EITHER_DOOR); });
		EventManager.Sub(Globals.Events.LOCK_DAD_DOOR, () => { EventManager.Fire(Globals.Events.LOCK_EITHER_DOOR); });

		// (HAS_MOM_KEY || HAS_DAD_KEY) => PICKUP_EITHER_KEY
		EventManager.Sub(Globals.Events.HAS_MOM_KEY, () => { EventManager.Fire(Globals.Events.PICKUP_EITHER_KEY); });
		EventManager.Sub(Globals.Events.HAS_DAD_KEY, () => { EventManager.Fire(Globals.Events.PICKUP_EITHER_KEY); });

		// HAS_MILLIE_KEY => GO_CHECK_MUSIC_BOX
		EventManager.Sub(Globals.Events.HAS_MILLIE_KEY, () => { EventManager.Fire(Globals.Events.GO_CHECK_MUSIC_BOX); });

	}

	public static void OnExit()
	{
		forks.Clear();
	}
}
