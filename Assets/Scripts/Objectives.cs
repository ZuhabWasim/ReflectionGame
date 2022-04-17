using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective
{
	public readonly string description;
	public readonly string activateEvent;
	public readonly string completedEvent;
	public bool isActive { get; set; }
	public readonly List<Objective> subObjectives;
	private float activateTime = 0.0f; // can give player some time before the objective shows up
	public Coroutine activationDelayCoroutine { get; private set; }

	public Objective( string desc, string activateEv = "", string completeEvent = "", bool isActive = false, float postActivateDelay = 0.0f )
	{
		this.description = desc;
		this.activateEvent = activateEv;
		this.completedEvent = completeEvent;
		this.activateTime = postActivateDelay;
		this.subObjectives = new List<Objective>();

		if ( isActive ) activationDelayCoroutine = Utilities.CoroutineRunner.RunCoroutine( ActivateObjective() );
		else if ( activateEvent != string.Empty )
		{
			EventManager.Sub( activateEvent, () =>
			{
				activationDelayCoroutine = Utilities.CoroutineRunner.RunCoroutine( ActivateObjective() );
			} );
		}


		if ( completedEvent != string.Empty )
		{
			EventManager.Sub( completedEvent, () =>
			{
				this.isActive = false;
				for ( int i = 0; i < this.subObjectives.Count; i++ )
				{
					this.subObjectives[ i ].isActive = false;
				}
				OnCompleted();
			} );
		}
	}

	public void AddSubObjective( Objective obj )
	{
		if ( subObjectives.Contains( obj ) ) return;
		subObjectives.Add( obj );
	}

	public void OnCompleted()
	{
		Debug.LogFormat( "Completed objective: '{0}'", description );
	}
	public void OnActivated()
	{
		Debug.LogFormat( "New Objective: {0}", description );
		activationDelayCoroutine = null;
	}

	private IEnumerator ActivateObjective()
	{
		yield return new WaitForSecondsRealtime( this.activateTime );
		this.isActive = true;
		OnActivated();
	}
}

public class Objectives
{
	static List<Objective> m_objectives = new List<Objective>();

	public static void PopulateObjectives()
	{
		InitObjectives.InitAllObjectives( m_objectives );
	}

	public static Objective GetCurrentObjective()
	{
		// assuming that there is only one master objective active at a time
		foreach ( Objective obj in m_objectives )
		{
			if ( obj.isActive ) return obj;
		}

		return null;
	}

	public static void OnExit()
	{
		foreach ( Objective objective in m_objectives )
		{
			objective.isActive = false;
			if ( objective.activationDelayCoroutine is null )
			{
				continue;
			}
			Utilities.CoroutineRunner.StopRunningCoroutine( objective.activationDelayCoroutine );
		}

		m_objectives.Clear();
		// PopulateObjectives should be called when the game starts again
	}
}
