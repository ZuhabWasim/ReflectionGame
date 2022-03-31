using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MusicBox : InteractableAbstract
{

	private const string MUSICBOX_AUDIO_SOURCE = "MusicBoxAudioSource";
	public bool discoveredBox;

	public ProximityTrigger proximityTrigger;
	public GameObject pointLight;

	private bool hasMillieKey;
	private bool hasMomKey;
	private bool hasDadKey;
	private int keys = 0;

	private bool millieKeyInteracted;

	public MirrorInteractable pastFireplaceMirror;
	public MirrorInteractable presentFireplaceMirror;
	
	protected override void OnStart()
	{
		EventManager.Sub( Globals.Events.LIGHTS_TURN_ON, OnLightsTurningOn );
		EventManager.Sub( Globals.Events.HAS_MILLIE_KEY, OnHavingMillieKey );
		EventManager.Sub( Globals.Events.HAS_MOM_KEY, OnHavingMomKey );
		EventManager.Sub( Globals.Events.HAS_DAD_KEY, OnHavingDadKey );
		EventManager.Sub( Globals.Events.FIRST_TELEPORT, OnFirstTeleport );
		
		// AudioPlayer.RegisterAudioPlayer(MUSICBOX_AUDIO_SOURCE, GetComponent<AudioSource>());

		discoveredBox = false;

		hasMillieKey = false;
		hasMomKey = false;
		hasDadKey = false;

		millieKeyInteracted = false;
		
		pastFireplaceMirror = GameObject.FindGameObjectWithTag(Globals.Tags.PAST_FIREPLACE_MIRROR)
			.GetComponent<MirrorInteractable>();
	}

	protected override void OnUserInteract()
	{
		discoveredBox = true;
		
		// Has all three keys
		if (HasAllKeys())
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.HAVING_ALL_KEYS_AUDIO, Globals.Tags.DIALOGUE_SOURCE);
		}
		// On completing Mom's closet
		else if (hasMomKey)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.HAVING_MOM_KEY_AUDIO, Globals.Tags.DIALOGUE_SOURCE);
		}
		// On completing Dad's closet
		else if (hasDadKey)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.HAVING_DAD_KEY_AUDIO, Globals.Tags.DIALOGUE_SOURCE);
		}
		// Millie's key
		else if (hasMillieKey)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.HAVING_MILLIE_KEY_AUDIO, Globals.Tags.DIALOGUE_SOURCE);
		}
		// Initial interaction dialogue
		else
		{
			AudioPlayer.Play(Globals.VoiceLines.Section1.ITS_MY_MUSICBOX, Globals.Tags.DIALOGUE_SOURCE);
		}
	}

	// Handling OnUseItem() differently than the class intended, since these items are used in a special way.
	public override void OnUseItemUnfiltered()
	{
		Inventory inventory = Inventory.GetInstance();
		PickupItem selectedItem = inventory.GetSelectedPickupItem();
		
		// Not holding anything
		if (selectedItem == null)
		{
			AudioPlayer.Play(Globals.VoiceLines.General.NOT_HOLDING_ANYTHING, Globals.Tags.DIALOGUE_SOURCE);
			return;
		}
		
		// Check and use the keys.
		if (selectedItem.itemName == Globals.UIStrings.MUSICBOXMILLIE_ITEM)
		{
			if (!millieKeyInteracted)
			{
				AudioPlayer.Play(Globals.VoiceLines.Section1.BOX_NEEDS_TWO_OTHER_KEYS, Globals.Tags.DIALOGUE_SOURCE);
				EventManager.Fire( Globals.Events.MILLIE_KEY_INTERACT );
				millieKeyInteracted = true;
				Debug.Log( "Millie Key Interact" );
				return;
			}

			if (!HasAllKeys())
			{
				AudioPlayer.Play(Globals.VoiceLines.Section4.FIND_ALL_THREE_KEYS, Globals.Tags.DIALOGUE_SOURCE);
				return;
			}
			
			keys -= 1;
			inventory.DeleteItem( selectedItem.itemName );
		}
		else if (selectedItem.itemName == Globals.UIStrings.MUSICBOXMOM_ITEM ||
		         selectedItem.itemName == Globals.UIStrings.MUSICBOXDAD_ITEM)
		{
			if (!HasAllKeys())
			{
				AudioPlayer.Play(Globals.VoiceLines.Section4.FIND_ALL_THREE_KEYS, Globals.Tags.DIALOGUE_SOURCE);
				return;
			}
			keys -= 1;
			inventory.DeleteItem( selectedItem.itemName );
		}
		else
		{
			Debug.Log("I can't use this here");
			AudioPlayer.Play(Globals.VoiceLines.General.CANT_USE_ITEM, Globals.Tags.DIALOGUE_SOURCE);
			return;
		}
		
		// If we got here, the player used a key.
		if (keys == 2)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.THATS_ONE, Globals.Tags.DIALOGUE_SOURCE);
		} else if (keys == 1)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section4.THATS_TWO, Globals.Tags.DIALOGUE_SOURCE);
		} else if (keys == 0)
		{
			//AudioPlayer.Play(Globals.VoiceLines.Section4.THATS_THREE, Globals.Tags.DIALOGUE_SOURCE, false);
			OpenMusicBox();
		}
		
	}

	void OpenMusicBox()
	{
		Debug.Log("Opening Music Box");
		AudioPlayer.Play(Globals.VoiceLines.Section4.MUSIC_BOX_OPENED, Globals.Tags.DIALOGUE_SOURCE);

		InterpolateTransform it = GetComponentInChildren<InterpolateTransform>();
		it.TriggerMotion();
	}

	void OnLightsTurningOn()
	{
		pointLight.SetActive(true);
	}
	void OnHavingMillieKey()
	{
		if ( !hasMillieKey ) keys += 1;
		hasMillieKey = true;
		Debug.Log("Keys = " + keys);
		proximityTrigger.enabled = true;
	}

	void OnHavingMomKey()
	{
		if ( !hasMomKey ) keys += 1;
		hasMomKey = true;
		Debug.Log("Keys = " + keys);
		DisableFireplaceMirrorTeleport();
	}

	void OnHavingDadKey()
	{
		if ( !hasDadKey ) keys += 1;
		hasDadKey = true;
		Debug.Log("Keys = " + keys);
	}

	void OnFirstTeleport()
	{
		Debug.Log("OnFirstTeleport: " + presentFireplaceMirror);
		// Removes the initial voice line.
		if (presentFireplaceMirror != null)
		{
			presentFireplaceMirror.teleportingVoiceLine = null;
		}
	}

	private void DisableFireplaceMirrorTeleport()
	{
		// Player can no longer go into the past after they collect all three keys.
		if (keys == 3 && presentFireplaceMirror != null)
		{
			presentFireplaceMirror.teleportable = false;
		}
	}
	private bool HasAllKeys()
	{
		return hasMillieKey && hasDadKey && hasMomKey;
	}
}
