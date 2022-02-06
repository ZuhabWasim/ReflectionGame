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
            List<System.Action> callbacks = ( (List<System.Action>) m_arglessListeners[ev] );
            foreach ( System.Action cb in callbacks )
            {
                cb();
            }
        }

        if ( !m_argListeners.ContainsKey( ev ) )
        {
            return;
        }

        List<System.Action<GameObject>> argcallbacks = ( (List<System.Action<GameObject>>) m_argListeners[ev] );
        foreach ( System.Action<GameObject> cb in argcallbacks )
        {
            cb( obj );
        }
    }

    public static void Sub( string ev, System.Action callback )
    {
        if ( !m_arglessListeners.ContainsKey( ev ) )
        {
            m_arglessListeners.Add( ev, new List<System.Action>() );
        }

        List<System.Action> callbacks = ( (List<System.Action>) m_arglessListeners[ev] );
        if ( !callbacks.Contains( callback ) )
        {
            callbacks.Add( callback );
        }
    }

    public static void Sub( string ev, System.Action<GameObject> callback )
    {
        if ( !m_argListeners.ContainsKey( ev ) )
        {
            m_argListeners.Add( ev, new List<System.Action<GameObject>>() );
        }

        List<System.Action<GameObject>> callbacks = ( (List<System.Action<GameObject>>) m_argListeners[ev] );
        if ( !callbacks.Contains( callback ) )
        {
            callbacks.Add( callback );
        }
    }

    public static void Unsub( string ev, System.Action callback )
    {
        if ( !m_arglessListeners.ContainsKey( ev ) )
        {
            return;
        }

        ( (List<System.Action>) m_arglessListeners[ev] ).Remove( callback );
    }

    public static void Unsub( string ev, System.Action<GameObject> callback )
    {
        if ( !m_argListeners.ContainsKey( ev ) )
        {
            return;
        }

        ( (List<System.Action<GameObject>>) m_arglessListeners[ev] ).Remove( callback );
    }
}
