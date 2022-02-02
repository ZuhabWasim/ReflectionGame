using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: This should not be initialized
public class GlobalState : MonoBehaviour
{
    private static Hashtable m_globalVars = new Hashtable();

    public static void AddVar<T>( string name, T initValue )
    {
        if ( m_globalVars.Contains( name ) )
        {
            Debug.LogFormat( "Tried to init global state var {0} that already exists, ignoring..", name );
        }

        m_globalVars.Add( name, initValue );
    }

    public static T GetVar<T>( string name )
    {
        if ( m_globalVars.Contains( name ) )
        {
            return (T) m_globalVars[ name ];
        }
        else
        {
            Debug.LogWarningFormat( "Tried to access global state var {0} that doesn't exist", name );
            return default( T );
        }
    }

    public static void SetVar<T>( string name, T newVal )
    {
        if ( m_globalVars.Contains( name ) )
        {
            m_globalVars[ name ] = newVal;
        }
        else
        {
            Debug.LogWarningFormat( "Tried to update global state var {0} that doesn't exist, ignoring..", name );
        }
    }
}
