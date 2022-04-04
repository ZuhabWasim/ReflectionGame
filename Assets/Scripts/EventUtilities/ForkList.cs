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

	[RuntimeInitializeOnLoadMethod]
	static void InitForks()
	{
		forks = new List<Fork>();

		// (GO_CLEAN_MIRROR && GO_ENTER_CODE) => DONE_INVESTIGATE
		forks.Add( new Fork( new string[] { Globals.Events.GO_CLEAN_MIRROR,
			Globals.Events.GO_ENTER_CODE },
			Globals.Events.DONE_INVESTIGATE ) );

	}

}
