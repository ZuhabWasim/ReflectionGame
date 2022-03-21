using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct RegisteredKeybind
{
	public readonly KeyCode key;
	bool isPressed;
	public readonly string keydownEvent;
	public readonly string keyupEvent;

	public bool IsPressed()
	{
		return isPressed;
	}

	public void SetPressed( bool pressed )
	{
		isPressed = pressed;
	}

	public RegisteredKeybind( KeyCode k )
	{
		key = k;
		isPressed = false;
		keydownEvent = key.ToString() + "Down";
		keyupEvent = key.ToString() + "Up";
	}
}

public class InputManager : MonoBehaviour
{
	private static Hashtable m_registeredKeybinds = new Hashtable();

	void Awake()
	{
		RegisterKeybinds();
	}

	void RegisterKeybinds()
	{
		Keybinds[] keybinds = (Keybinds[])System.Enum.GetValues( typeof( Keybinds ) );
		foreach ( Keybinds key in keybinds )
		{
			RegisteredKeybind k = new RegisteredKeybind( (KeyCode)key );
			m_registeredKeybinds.Add( key, k );
		}
	}

	public static string GetKeyDownEventName( Keybinds key )
	{
		return ( (RegisteredKeybind)m_registeredKeybinds[ key ] ).keydownEvent;
	}

	public static string GetKeyUpEventName( Keybinds key )
	{
		return ( (RegisteredKeybind)m_registeredKeybinds[ key ] ).keyupEvent;
	}

	void Update()
	{
		foreach ( DictionaryEntry entry in m_registeredKeybinds )
		{
			RegisteredKeybind keybind = (RegisteredKeybind)entry.Value;
			if ( Input.GetKeyDown( keybind.key ) && !keybind.IsPressed() )
			{
				keybind.SetPressed( true );
				EventManager.Fire( keybind.keydownEvent );
			}
			else if ( Input.GetKeyUp( keybind.key ) )
			{
				keybind.SetPressed( false );
				EventManager.Fire( keybind.keyupEvent );
			}
		}
	}
}
