using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNote : InteractableAbstract
{

	public int id;
	public TextAsset noteText;
	public static InteractNote[] journal = {null, null, null, null, null};
	public static int journalSize = 5;
	public static int pointer = 0;
	public string title;
	protected override void OnUserInteract()
	{
		journal[id] = this;
		this.gameObject.SetActive(false);
		pointer = id;
		InventoryDisplay inventoryDisplay = GameObject.Find(Globals.Misc.UI_Canvas).GetComponent<InventoryDisplay>();
		inventoryDisplay.showNewEntryNotice();
		AudioPlayer.Play( Globals.AudioFiles.General.PAPER_UNRAVEL, Globals.Tags.MAIN_SOURCE );
		AudioPlayer.Play( Globals.VoiceLines.Section1.MILLE_POV_INTRO, Globals.Tags.DIALOGUE_SOURCE, false);
	}


}
