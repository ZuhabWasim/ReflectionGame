using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{

	public int id;
	public TextAsset noteText;
	public static InteractNote[] journal = { null, null, null, null, null };
	public static int journalSize = 5;
	public static int pointer = 0;
	public string title;
	public string onReadFireEvent;

	protected override void OnUserInteract()
	{
		journal[ id ] = this;
		this.gameObject.SetActive( false );
		pointer = id;
		AudioPlayer.Play( Globals.AudioFiles.General.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
		EventManager.Fire( InputManager.GetKeyDownEventName( Keybinds.ESCAPE_KEY ) );
	}
}
