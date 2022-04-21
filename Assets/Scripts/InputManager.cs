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

struct CheatCodeDetector
{
	private string userInput;
	private string cheatCode;
	private bool detected;

	public CheatCodeDetector( string cheatCode )
	{
		this.userInput = string.Empty;
		this.cheatCode = cheatCode;
		this.detected = false;
	}

	public void ProcessInput( string key )
	{
		if ( detected )
		{
			return;
		}
		this.userInput += key;
		if ( this.userInput[ this.userInput.Length - 1 ] != this.cheatCode[ this.userInput.Length - 1 ] )
		{
			this.userInput = string.Empty;
		}
		else if ( this.userInput.Equals( this.cheatCode ) )
		{
			detected = true;
			Debug.Log( "Cheat success" );
			EventManager.Fire( Globals.Events.CHEAT_SUCCESS );
		}
	}

	public void Reset()
	{
		this.detected = false;
		this.userInput = string.Empty;
	}
}

public class InputManager : MonoBehaviour
{
	private static Hashtable m_registeredKeybinds = new Hashtable();
	private static CheatCodeDetector m_keysCheatCodeDetector = new CheatCodeDetector( Globals.Misc.CHEAT_CODE );

	void Awake()
	{
		RegisterKeybinds();
		m_keysCheatCodeDetector.Reset();
	}

	void RegisterKeybinds()
	{
		Keybinds[] keybinds = (Keybinds[])System.Enum.GetValues( typeof( Keybinds ) );
		foreach ( Keybinds key in keybinds )
		{
			if ( m_registeredKeybinds.Contains( key ) ) continue;
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

	public static void HandleCheatInput( string inputKey )
	{
		m_keysCheatCodeDetector.ProcessInput( inputKey );
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
