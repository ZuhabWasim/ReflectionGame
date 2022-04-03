using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{

	public int id;
	public Texture noteText;
	public static List<InteractNote> journal = new List<InteractNote>();
	public static int pointer = 0;
	public string title;
	public string onReadFireEvent;

	protected override void OnUserInteract()
	{
		if ( pointer == 0 )
		{
			journal.Add( null );
		} 
		journal.Add(this);
		this.gameObject.SetActive( false );
		pointer += 1;
		AudioPlayer.Play( Globals.AudioFiles.General.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
		EventManager.Fire( InputManager.GetKeyDownEventName( Keybinds.TAB_KEY ) );
	}
}
