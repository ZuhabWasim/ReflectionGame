using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	private static Hashtable m_arglessListeners = new Hashtable();
	private static Hashtable m_argListeners = new Hashtable();

	public static void Fire( string ev, GameObject obj = null )
	{
		if ( m_arglessListeners.ContainsKey( ev ) )
		{
			List<System.Action> callbacks = ( (List<System.Action>)m_arglessListeners[ ev ] );
			foreach ( System.Action cb in callbacks )
			{
				try
				{
					cb();
				}
				catch ( System.Exception )
				{
					continue;
				}
			}
		}

		if ( !m_argListeners.ContainsKey( ev ) )
		{
			return;
		}

		List<System.Action<GameObject>> argcallbacks = ( (List<System.Action<GameObject>>)m_argListeners[ ev ] );
		foreach ( System.Action<GameObject> cb in argcallbacks )
		{
			try
			{
				cb( obj );
			}
			catch ( System.Exception )
			{
				continue;
			}
		}
	}

	public static void Sub( string ev, System.Action callback )
	{
		string[] substrings = ev.Split( ' ' );

		foreach ( string s in substrings )
		{
			if ( !m_arglessListeners.ContainsKey( s ) )
			{
				m_arglessListeners.Add( s, new List<System.Action>() );
			}

			List<System.Action> callbacks = ( (List<System.Action>)m_arglessListeners[ s ] );
			if ( !callbacks.Contains( callback ) )
			{
				callbacks.Add( callback );
			}
		}
	}

	public static void Sub( string ev, System.Action<GameObject> callback )
	{
		string[] substrings = ev.Split( ' ' );

		foreach ( string s in substrings )
		{
			if ( !m_argListeners.ContainsKey( s ) )
			{
				m_argListeners.Add( s, new List<System.Action<GameObject>>() );
			}

			List<System.Action<GameObject>> callbacks = ( (List<System.Action<GameObject>>)m_argListeners[ s ] );
			if ( !callbacks.Contains( callback ) )
			{
				callbacks.Add( callback );
			}
		}
	}

	public static void Unsub( string ev, System.Action callback )
	{
		if ( !m_arglessListeners.ContainsKey( ev ) )
		{
			return;
		}

		( (List<System.Action>)m_arglessListeners[ ev ] ).Remove( callback );
	}

	public static void Unsub( string ev, System.Action<GameObject> callback )
	{
		if ( !m_argListeners.ContainsKey( ev ) )
		{
			return;
		}

		( (List<System.Action<GameObject>>)m_arglessListeners[ ev ] ).Remove( callback );
	}

	public static void OnExit()
	{
		m_argListeners.Clear();
		m_arglessListeners.Clear();
	}
}
